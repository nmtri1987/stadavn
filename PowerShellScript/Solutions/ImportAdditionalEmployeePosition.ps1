Param($SiteURL,$FilePath);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

# enable event firing 
$myAss = [Reflection.Assembly]::LoadWithPartialName("Microsoft.SharePoint");
$type = $myAss.GetType("Microsoft.SharePoint.SPEventManager");
$prop = $type.GetProperty([string]"EventFiringDisabled",[System.Reflection.BindingFlags] ([System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Static));
$prop.SetValue($null, $false, $null);

if($SiteURL -eq $null){
	$SiteURL="http://localhost"
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main($SiteURL,$FilePath){

	if($FilePath -eq $null)
	{
		$FilePath = $currentDirectory + "\CSVFile\AdditionalEmployeePosition.csv"
	}
		
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		Write-Host $SiteURL -ForegroundColor DarkBlue
		$web = Get-SPWeb $SiteURL
		$addEmployeePosCsvFilePath = get-item $filePath
		Write-Host "Path of file :: " $addEmployeePosCsvFilePath
		Write-Host "Reading " $addEmployeePosCsvFilePath " file" -ForegroundColor Green

		$addEmployeePosList = $web.Lists["Additional Employee Position"]
		$employeesList = $web.Lists["Employees"]
		foreach($item in Import-Csv $addEmployeePosCsvFilePath)
		{
			$isExistedId = CheckIfCodeExist $addEmployeePosList $item
			Write-Host $item.EmployeeID
			if($isExistedId -ne 0)
			{
				#Write-Host "Update Employee: " $item.EmployeeID
				#UpdateAddtionalEmployeePosition $web $addEmployeePosList $isExistedId $item
				Write-Host "Existed" -ForegroundColor Green
			}
			else
			{
				InsertAddtionalEmployeePosition $web $addEmployeePosList $item $employeesList
			}
						
			Write-Host "Please wait ...." -ForegroundColor DarkMagenta
		}

		$web.Dispose()
	}
}

function CheckIfCodeExist($list, $itemData)
{
	$qryCode = new-object Microsoft.SharePoint.SPQuery
	$qryCode.Query = "<Where><And><And><Eq><FieldRef Name='EmployeeID' /><Value Type='Text'>"+$itemData.EmployeeID+"</Value></Eq><Eq><FieldRef Name='Module' /><Value Type='Choice'>"+$itemData.Module+"</Value></Eq></And><Eq><FieldRef Name='EmployeeLevel' /><Value Type='Number'>"+$itemData.EmployeeLevel+"</Value></Eq></And></Where>"
	$dataResults = $list.getItems($qryCode)
	if($dataResults.Count -eq 0)
	{
		return 0
	}
	else
	{
		return $dataResults[0].ID
	}
}

function GetEmployeeByEmployeeID($list, $employeeId)
{
	$qryCode = new-object Microsoft.SharePoint.SPQuery
	$qryCode.Query = "<Where><Eq><FieldRef Name='EmployeeID' /><Value Type='Text'>"+$employeeId+"</Value></Eq></Where>"
	$dataResults = $list.getItems($qryCode)
	if($dataResults.Count -eq 0)
	{
		return $null
	}
	else
	{
		return $dataResults[0]
	}
}

function UpdateAddtionalEmployeePosition($web, $list, $itemId, $itemData)
{
	try{
		$itemToUpdate = $list.GetItemById($itemId)

		$itemToUpdate["Module"] = $itemData.Module
		$itemToUpdate["EmployeeLevel"] = $itemData.EmployeeLevel
		
		$itemToUpdate.Update()
		Write-Host "Updated successfully" -ForegroundColor Green
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Update AdditionalEmployeePosition $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\UpdateAddtionalEmployeePosition.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}

function InsertAddtionalEmployeePosition($web,$list, $itemData, $employeesList)
{
	try{
		if($itemData.EmployeeID -ne $null -and $itemData.EmployeeID -ne "")
		{
			$employee = GetEmployeeByEmployeeID $employeesList $itemData.EmployeeID
			if($employee -ne $null)
			{
				$newItem = $list.Items.Add()
				$newItem["Employee"] = $employee.Id
				$newItem["Module"] = $itemData.Module
				$newItem["EmployeeLevel"] = $itemData.EmployeeLevel
				$newItem.Update()
				Write-Host "Inserted successfully" -ForegroundColor Green
			}
		}
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Insert AddtionalEmployeePosition $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\InsertAddtionalEmployeePosition.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}

Start-Transcript  
Write-Host "START IMRPORT ADDITIONAL EMPLOYEE POSITION DATA" -ForegroundColor Blue
Main $SiteURL $FilePath
Write-Host "WAITING FOR ADDITIONAL EMPLOYEE POSITION..." -ForegroundColor Blue
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell