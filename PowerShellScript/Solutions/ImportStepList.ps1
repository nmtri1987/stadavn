Param($SiteURL);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
   Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}
set-executionpolicy remotesigned

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main($SiteURL)
{
	Write-Host $SiteURL -ForegroundColor DarkGreen
	$filePath = ".\CSVFile\StepList.csv"
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		$web = Get-SPWeb $SiteURL
		$stepListCSVFilePath = get-item $filePath
		Write-Host "Path of file :: " $stepListCSVFilePath
		Write-Host "Reading " $stepListCSVFilePath " file" -ForegroundColor Green
		$stepList = $web.Lists["Step List"]
		$empPositionList = $web.Lists["Employee Position"]
		$departmentList = $web.Lists["Departments"]
		
		RemoveAll $stepList
		
		foreach($item in Import-Csv $stepListCSVFilePath)
		{
			InsertStepListData $stepList $item $departmentList $empPositionList
		}
		$web.Dispose()
	}
}

function RemoveAll($list)
{
	$dataResults = $list.Items
	
	if($dataResults.Count -gt 0)
	{
		$listItemsTotal = $dataResults.Count
		for ($x=$listItemsTotal-1;$x -ge 0; $x--)
		{
			$dataResults[$x].Delete()
		}
	}
}

function InsertStepListData($list, $itemData, $departmentList, $empPositionList)
{
	try {
		$newItem = $list.Items.Add()
		$newItem["StepModule"] = $itemData.StepModule
		$newItem["CurrentStepStatus"] = $itemData.CurrentStepStatus
		$newItem["StepNumber"] =  $itemData.StepNumber
		$newItem["CommonDepartment"] = $itemData.CommonDepartment
		
		if($itemData.CommonDepartment -eq '')
		{
			$newItem["CommonDepartment"] = $itemData.CommonDepartment
		}
		else
		{			
			$queryDepartment = New-Object  Microsoft.SharePoint.SPQuery 
			$queryDepartment.query ="<Where><Eq><FieldRef Name='Code' /><Value Type='Text'>"+$itemData.CommonDepartment+"</Value></Eq></Where>"
			$departments = $departmentList.getItems($queryDepartment)
			if ($departments -ne $null -and $departments.Count -gt 0)
			{
				$newItem["CommonDepartment"] = $departments[0].ID
			}
		}
		
		$queryEmpPos = New-Object  Microsoft.SharePoint.SPQuery 
		$queryEmpPos.query ="<Where><Eq><FieldRef Name='Code' /><Value Type='Text'>"+$itemData.StepPosition+"</Value></Eq></Where>"
		$items = $empPositionList.getItems($queryEmpPos)
		if ($items.Count -gt 0)
		{
			$newItem["StepPosition"] = $items[0].ID
		}
		
		$newItem.Update()
		Write-Host "Inserted successfully" -ForegroundColor Green
	}
	catch
    {
		$currentDate = Get-Date
		"$currentDate : Error - Insert Data $list $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportStepList.txt" -Append;   
		Write-host "Error - Insert Data $list $itemData : $_"  -ForegroundColor Red
	}
}

Start-Transcript  
Write-Host "START IMPORT DATA TO STEP LIST" -ForegroundColor Blue
Main $SiteURL 
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript

Remove-PsSnapin Microsoft.SharePoint.PowerShell