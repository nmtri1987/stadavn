Param($SiteURL);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

if($SiteURL -eq $null){
	$SiteURL="http://localhost"
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main($SiteURL){

	if($FilePath -eq $null)
	{
		$FilePath = $currentDirectory + "\CSVFile\WorkflowStepData.csv"
	}
		
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		Write-Host $SiteURL -ForegroundColor DarkBlue
		$web = Get-SPWeb $SiteURL
		$workflowStepCsvFilePath = get-item $filePath
		Write-Host "Path of file :: " $departmentCsvFilePath
		Write-Host "Reading " $workflowStepCsvFilePath " file" -ForegroundColor Green

		$workflowStepList = $web.Lists["Workflow steps"]
		foreach($item in Import-Csv $workflowStepCsvFilePath)
		{
			$isExistedId = CheckIfTitleExist $workflowStepList $item.Title
			Write-Host $item.Title
			if($isExistedId -ne 0)
			{
				Write-Host "Update Step: " $item.Title
				UpdateWSStep $web $workflowStepList $isExistedId $item
			}
			else
			{
                Write-Host "Insert Step: " $item.Title
				InsertWSStep $web $workflowStepList $item
			}
		}
		$web.Dispose()
	}
}

function CheckIfTitleExist($list, $title)
{
	$qryCode = new-object Microsoft.SharePoint.SPQuery
	$qryCode.Query = "<Where><Eq> <FieldRef Name='Title' /><Value Type='Text'>"+$title+"</Value></Eq></Where>"
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

function UpdateWSStep($web, $list, $itemId, $itemData)
{
	try{
        $positionList = $web.Lists["Employee Position"]
        $employeeList = $web.Lists["Employees"]
		$itemToUpdate = $list.GetItemById($itemId)

		$itemToUpdate["Title"] = $itemData.Title
		$itemToUpdate["ListName"] = $itemData.ListName

		$itemToUpdate["CurrentStep"] = Get-GroupPositionString  $positionList $itemData.CurrentStep 
		$itemToUpdate["NextStep"] = Get-GroupPositionString  $positionList $itemData.NextStep		
		$itemToUpdate["NotificationEmailToRoles"] = Get-GroupPositionString  $positionList $itemData.NotificationEmailToRoles
		$itemToUpdate["NotificationEmailCcRoles"] = Get-GroupPositionString  $positionList $itemData.NotificationEmailCcRoles
		$itemToUpdate["AllowReject"] = $itemData.AllowReject
		$itemToUpdate["NotificationEmailToEmployees"] = Get-GroupEmployeeString  $employeeList $itemData.NotificationEmailToEmployees
		$itemToUpdate["NotificationEmailCcEmployees"] = Get-GroupEmployeeString  $employeeList $itemData.NotificationEmailCcEmployees

		if($itemData.ConditionalExpression -ne $null -AND $itemData.ConditionalExpression -ne "")
		{
			$itemToUpdate["ConditionalExpression"] = $itemData.ConditionalExpression
		}
		$itemToUpdate["OrderStep"] = $itemData.OrderStep

		$itemToUpdate.Update()
		Write-Host "Updated successfully" -ForegroundColor Green
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Update department $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportWorkflowStepList.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}

function InsertWSStep($web, $list, $itemData)
{
	try{
		$positionList = $web.Lists["Employee Position"]
        $employeeList = $web.Lists["Employees"]
		$newItem = $list.Items.Add()

		$newItem["Title"] = $itemData.Title
		$newItem["ListName"] = $itemData.ListName
	
		$newItem["CurrentStep"] = Get-GroupPositionString  $positionList $itemData.CurrentStep 
		$newItem["NextStep"] = Get-GroupPositionString  $positionList $itemData.NextStep	
		$newItem["NotificationEmailToRoles"] = Get-GroupPositionString  $positionList $itemData.NotificationEmailToRoles
		$newItem["NotificationEmailCcRoles"] = Get-GroupPositionString  $positionList $itemData.NotificationEmailCcRoles

        $newItem["NotificationEmailToEmployees"] = Get-GroupEmployeeString  $employeeList $itemData.NotificationEmailToEmployees
		$newItem["NotificationEmailCcEmployees"] = Get-GroupEmployeeString  $employeeList $itemData.NotificationEmailCcEmployees

		$newItem["AllowReject"] = $itemData.AllowReject
		
		if($itemData.ConditionalExpression -ne $null -AND $itemData.ConditionalExpression -ne "")
		{
			$newItem["ConditionalExpression"] = $itemData.ConditionalExpression
		}
		$newItem["OrderStep"] = $itemData.OrderStep
		$newItem.Update()
		Write-Host "Inserted successfully" -ForegroundColor Green
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Insert department $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportWorkflowStepList.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}


function Get-GroupPositionString($positionList, $positionString)
{
    if($positionString -eq $null -or $positionString -eq "")
    {
        return "";
    }
	if($positionString -Match ";")
	{
		$positions = $positionString.Split("{;}")
	}
	else
	{
		$positions = @($positionString) #Array 1 element
	}
    
	$itemValues = new-object Microsoft.SharePoint.SPFieldLookupValueCollection

	$positions | ForEach {
			$qry = new-object Microsoft.SharePoint.SPQuery
			$qry.Query = "<Where><Eq> <FieldRef Name='Code' /><Value Type='Text'>"+$_+"</Value></Eq></Where>"
			$positionData = $positionList.getItems($qry)
			#Write-Host $_ -ForegroundColor Green
			if($positionData.Count -gt 0)
			{
				$lookupValue = New-Object Microsoft.SharePoint.SPFieldLookupValue($positionData[0].ID,$positionData[0].ID);
				$itemValues.Add($lookupValue)
			}
	}
	return $itemValues.ToString()
}

function Get-GroupEmployeeString($employeeList, $employeeString)
{
    if($employeeString -eq $null -or $employeeString -eq "")
    {
        return "";
    }
	if($employeeString -Match ";")
	{
		$employees = $employeeString.Split("{;}")
	}
	else
	{
		$employees = @($employeeString) #Array 1 element
	}
    
	$itemValues = new-object Microsoft.SharePoint.SPFieldLookupValueCollection

	$employees | ForEach {
			$qry = new-object Microsoft.SharePoint.SPQuery
			$qry.Query = "<Where><Eq> <FieldRef Name='EmployeeID' /><Value Type='Text'>"+$_+"</Value></Eq></Where>"
			$employeeData = $employeeList.getItems($qry)
			#Write-Host $_ -ForegroundColor Green
			if($employeeData.Count -gt 0)
			{
				$lookupValue = New-Object Microsoft.SharePoint.SPFieldLookupValue($employeeData[0].ID,$employeeData[0].ID);
				$itemValues.Add($lookupValue)
			}
	}
	return $itemValues.ToString()
}

Start-Transcript  
Write-Host "START IMPORT WORKFLOW STEP LIST DATA" -ForegroundColor Blue
Main $SiteURL
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell