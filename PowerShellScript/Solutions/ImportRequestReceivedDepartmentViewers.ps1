Param($SiteURL);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
   Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}
set-executionpolicy remotesigned

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main-ImportRequestReceivedDepartmentViewers($SiteURL)
{
	Write-Host $SiteURL -ForegroundColor DarkGreen
	$filePath = ".\CSVFile\RequestReceivedDepartmentViewersData.csv"
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		$web = Get-SPWeb $SiteURL
		$viewersCsvFilePath = get-item $filePath
		Write-Host "Path of file :: " $viewersCsvFilePath
		Write-Host "Reading " $viewersCsvFilePath " file" -ForegroundColor Green
		$viewersList = $web.Lists["Request received department viewers"]
		$employeeList = $web.Lists["Employees"]
		$locationList = $web.Lists["Factories"]
		$departmentList = $web.Lists["Departments"]

		foreach($item in Import-Csv $viewersCsvFilePath)
		{
			$locationId = GetLocationByName $locationList $item.LocationName
			$departmentId = GetDepartmentByName $departmentList $item.DepartmentName
			if($locationId -gt 0 -and $departmentId -gt 0)
			{
					$isExistedId = CheckIfItemExist $viewersList $locationId $departmentId
			
					if($isExistedId -eq 0)
					{
						Write-Host "Inserting ... " -ForegroundColor Green
						InsertViewersData $viewersList $employeeList $item $locationId $departmentId
					}
					else
					{
						Write-Host "Updating ..." -ForegroundColor Yellow
						UpdateViewersData  $viewersList $employeeList $isExistedId $item 
					}
			}
			else 
			{
				if($locationId -eq 0)
				{
					Write-Host "Can not find location: " $item.LocationName
				}
				if($departmentId -eq 0)
				{
					Write-Host "Can not find department: " $item.DepartmentName
				}
			}
		
		}
		$web.Dispose()
	}
}


function Get-GroupEmployeesString($employeeList, $employeeIDString)
{
    $emps = $employeeIDString.Split("{;}")
	$itemValues =new-object Microsoft.SharePoint.SPFieldLookupValueCollection

	$emps | ForEach {
			$qry = new-object Microsoft.SharePoint.SPQuery
			$qry.Query = "<Where><Eq><FieldRef Name='EmployeeID' /><Value Type='Text'>"+$_+"</Value></Eq></Where>"
			$employeesData = $employeeList.getItems($qry)
			#Write-Host $_ -ForegroundColor Green
			if($employeesData.Count -gt 0)
			{
				$lookupValue = New-Object Microsoft.SharePoint.SPFieldLookupValue($employeesData[0].ID,$employeesData[0].ID);
				$itemValues.Add($lookupValue)
			}
	}
	return $itemValues.ToString()
}

function GetLocationByName($locationList, $name)
{
	$qryCode = new-object Microsoft.SharePoint.SPQuery
	$qryCode.Query = "<Where><Eq> <FieldRef Name='CommonName' /><Value Type='Text'>"+$name+"</Value></Eq></Where>"
	$dataResults = $locationList.getItems($qryCode)
	if($dataResults.Count -eq 0)
	{
		return 0
	}
	else
	{
		return $dataResults[0].ID
	}    
}

function GetDepartmentByName($departmentList, $name)
{
	$qryCode = new-object Microsoft.SharePoint.SPQuery
	$qryCode.Query = "<Where><Eq> <FieldRef Name='CommonName' /><Value Type='Text'>"+$name+"</Value></Eq></Where>"
	$dataResults = $departmentList.getItems($qryCode)
	if($dataResults.Count -eq 0)
	{
		return 0
	}
	else
	{
		return $dataResults[0].ID
	}    
}

function CheckIfItemExist($viewerList, $locationId, $departmentId)
{
	$qryCode = new-object Microsoft.SharePoint.SPQuery
	$qryCode.Query =  "<Where><And><Eq><FieldRef Name='CommonLocation' LookupId='TRUE' /><Value Type='Lookup'>"+$locationId+"</Value></Eq><Eq><FieldRef Name='CommonDepartment' LookupId='TRUE' /><Value Type='Lookup'>"+$departmentId+"</Value></Eq></And></Where>";
	$dataResults = $viewerList.getItems($qryCode)
	if($dataResults.Count -eq 0)
	{
		return 0
	}
	else
	{
		return $dataResults[0].ID
	}    
}


function InsertViewersData($viewersList, $employeeList,  $itemData, $locationId, $departmentId)
{
	try {
		$newItem = $viewersList.Items.Add()
		$newItem["Title"] =  $itemData.Title
		$newItem["CommonLocation"] = $locationId
		$newItem["CommonDepartment"] = $departmentId
		$newItem["Employees"] = Get-GroupEmployeesString $employeeList $itemData.Employees
		$newItem.Update()
		Write-Host "Inserted successfully" -ForegroundColor Green
	}
	catch
    {
		$currentDate = Get-Date
		"$currentDate : Error - Insert Data $list $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportRequestReceivedDepartmentViewers.txt" -Append;   
		Write-host "Error - Insert Data $list $itemData : $_"  -ForegroundColor Red
	}
}

function UpdateViewersData($viewersList,$employeeList,  $itemId, $itemData)
{
	try 
	{
		$itemToUpdate = $viewersList.GetItemById($itemId)
        $itemToUpdate["Employees"] = Get-GroupEmployeesString $employeeList $itemData.Employees
		$itemToUpdate["Title"] = $itemData.Title
		$itemToUpdate.Update()
		Write-Host "Updated successfully" -ForegroundColor Green
	}
	catch
    {
		$currentDate = Get-Date
		"$currentDate : Error - UpdateData $list $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportRequestReceivedDepartmentViewers.txt" -Append;   
		Write-host "Error - UpdateData $list $itemData : $_" -ForegroundColor Red
	}
}

Start-Transcript  
Write-Host "START IMPORT REQUEST RECEIVED DEPARTMENT VIEWERS" -ForegroundColor Blue
Main-ImportRequestReceivedDepartmentViewers $SiteURL 
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript

Remove-PsSnapin Microsoft.SharePoint.PowerShell