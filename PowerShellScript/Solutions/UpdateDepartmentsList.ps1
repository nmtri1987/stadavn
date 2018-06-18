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
		$FilePath = $currentDirectory + "\CSVFile\DepartmentList.csv"
	}
		
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		Write-Host $SiteURL -ForegroundColor DarkBlue
		$web = Get-SPWeb $SiteURL
		$departmentCsvFilePath = get-item $filePath
		Write-Host "Path of file :: " $departmentCsvFilePath
		Write-Host "Reading " $departmentCsvFilePath " file" -ForegroundColor Green

		$derpartmentList = $web.Lists["Departments"]
		foreach($item in Import-Csv $departmentCsvFilePath)
		{
			$isExistedId = CheckIfCodeExist $derpartmentList $item.Code
			Write-Host $item.Code
			if($isExistedId -ne 0)
			{
				Write-Host "Update department: " $item.CommonName
				UpdateDepartment $web $derpartmentList $isExistedId $item
			}
						
			Write-Host "Please wait ...." -ForegroundColor DarkMagenta
		}

		$web.Dispose()
	}
}

function CheckIfCodeExist($list, $code)
{
	$qryCode = new-object Microsoft.SharePoint.SPQuery
	$qryCode.Query = "<Where><Eq> <FieldRef Name='Code' /><Value Type='Text'>"+$code+"</Value></Eq></Where>"
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

function UpdateDepartment($web, $list, $itemId, $itemData)
{
	try{

		$itemToUpdate = $list.GetItemById($itemId)
		$itemToUpdate["AutoCreateOverTime"] = $itemData.AutoCreateOverTime

		$itemToUpdate.Update()
		Write-Host "Updated successfully" -ForegroundColor Green
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Update department $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportDepartment.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}

Start-Transcript  
Write-Host "START UPDATE DEPARTMENT DATA" -ForegroundColor Blue
Main $SiteURL $FilePath
Write-Host "WAITING FOR DEPARTMENT..." -ForegroundColor Blue
Write-Host "UPDATE DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell