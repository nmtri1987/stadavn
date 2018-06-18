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
	$filePath = ".\CSVFile\ShiftTimeData.csv"
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		$web = Get-SPWeb $SiteURL
		$shiftTimeCsvFilePath = get-item $filePath
		Write-Host "Path of file :: " $shiftTimeCsvFilePath
		Write-Host "Reading " $shiftTimeCsvFilePath " file" -ForegroundColor Green

		$shiftTimeList = $web.Lists["Shift Time"]

		foreach($item in Import-Csv $shiftTimeCsvFilePath)
		{
			$isExistedId = CheckIfCodeExist $shiftTimeList $item.Code
			if($isExistedId -eq 0)
			{
				Write-Host "Inserting..." -ForegroundColor Green
				InsertShiftTimeData $shiftTimeList $item
				
			}
			else
			{
				Write-Host "Updating..." -ForegroundColor Yellow
				UpdateShiftTimeData  $shiftTimeList $isExistedId $item
			}
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


function InsertShiftTimeData($list, $itemData)
{
	try {
		$newItem = $list.Items.Add()
		$newItem["CommonName"] = $itemData.Name
		$newItem["Code"] = $itemData.Code
		$newItem["ShiftTimeWorkingHourFrom"] = $itemData.WorkingHourFrom
		$newItem["ShiftTimeWorkingHourTo"] =  $itemData.WorkingHourTo
        if($itemData.WorkingHourMid.length -gt 0)
        {
            $newItem["ShiftTimeWorkingHourMid"] = $itemData.WorkingHourMid
        }
		$newItem["ShiftTimeBreakingHourFrom"] = $itemData.BreakingHourFrom
        $newItem["ShiftTimeBreakingHourTo"] = $itemData.BreakingHourTo 
		$newItem["ShiftTimeWorkingHour"] = $itemData.WorkingHourNumber
		$newItem["ShiftTimeBreakingHour"] = $itemData.BreakingHourNumber
		$newItem["UnexpectedLeaveFirstApprovalRole"] = $itemData.UnexpectedLeaveFirstApprovalRole
		$newItem["ShiftRequired"] = $itemData.ShiftRequired
		$newItem.Update()
		Write-Host "Inserted successfully" -ForegroundColor Green
	}
	catch
    {
		$currentDate = Get-Date
		"$currentDate : Error - InsertData $list $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportShiftTimeData.txt" -Append;   
		Write-host " Error - InsertData $list $itemData : $_" -ForegroundColor Red
	}
}

function UpdateShiftTimeData($list, $itemId, $itemData)
{
	try 
	{
		$itemToUpdate = $list.GetItemById($itemId)
		$itemToUpdate["CommonName"] = $itemData.Name
		$itemToUpdate["ShiftTimeWorkingHourFrom"] = $itemData.WorkingHourFrom
		$itemToUpdate["ShiftTimeWorkingHourTo"] =  $itemData.WorkingHourTo
        if($itemData.WorkingHourMid.length -gt 0)
        {
            $itemToUpdate["ShiftTimeWorkingHourMid"] = $itemData.WorkingHourMid
        }
		$itemToUpdate["ShiftTimeBreakingHourFrom"] = $itemData.BreakingHourFrom
		$itemToUpdate["ShiftTimeBreakingHourTo"] =  $itemData.BreakingHourTo
		$itemToUpdate["ShiftTimeWorkingHour"] = $itemData.WorkingHourNumber
		$itemToUpdate["ShiftTimeBreakingHour"] = $itemData.BreakingHourNumber
		$itemToUpdate["UnexpectedLeaveFirstApprovalRole"] = $itemData.UnexpectedLeaveFirstApprovalRole
		$itemToUpdate["ShiftRequired"] = $itemData.ShiftRequired
		$itemToUpdate.Update()
		Write-Host "Updated successfully" -ForegroundColor Green
	}
	catch
    {
		$currentDate = Get-Date
		"$currentDate : Error - UpdateData $list $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportShiftTimeData.txt" -Append;   
		Write-host "Error : $_" -ForegroundColor Red
	}
}

Start-Transcript  
Write-Host "START IMPORT DATA TO SHIFT TIME" -ForegroundColor Blue
Main $SiteURL 
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript

Remove-PsSnapin Microsoft.SharePoint.PowerShell