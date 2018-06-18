Param($SiteURL);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
   Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}
set-executionpolicy remotesigned
if ($SiteURL -eq $null) 
{ 
	$SiteURL = "http://localhost:81"
}
$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main($SiteURL)
{
	$filePath = ".\CSVFile\CalendarData.csv"
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		$web = Get-SPWeb $SiteURL
		$calendarCsvFilePath = get-item $filePath
		Write-Host "Path of file :: " $calendarCsvFilePath
		Write-Host "Reading " $calendarCsvFilePath " file" -ForegroundColor Green

		$calendarLocation1 = $web.Lists["Company Calendar - Location 1"]
		$calendarLocation2 = $web.Lists["Company Calendar - Location 2"]

		$lineNumber = 2
		#Insert data
		foreach($item in Import-Csv $calendarCsvFilePath)
		{
			if($item.Location -eq 1)
			{
				#Import to calendar location 1
				ImportDataToList $calendarLocation1 $item $lineNumber
			}
			else
			{
				#Import to calendar location 2
				ImportDataToList $calendarLocation2 $item $lineNumber
			}
			#Write-Host 	$item.Location
			$lineNumber = $lineNumber + 1
		}
	}
}

function ImportDataToList($list, $itemData, $lineNumber)
{
	try {
		Write-Host "====================================================================="

		Write-Host " "
		#InsertData $list $itemData
		#2017-02-26T14:00:00Z
		$startDateTime = $itemData.StartDate + "T" + $itemData.StartTime + "Z"
		$endDateTime = $itemData.EndDate + "T" + $itemData.EndTime + "Z"

		$qry = new-object Microsoft.SharePoint.SPQuery
		$qry.Query = "<Where><And><Eq><FieldRef Name='EventDate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>$startDateTime</Value></Eq><Eq><FieldRef Name='EndDate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>$endDateTime</Value></Eq></And></Where>"
		$itemResults = $list.GetItems($qry)

		if($itemResults.Count -gt 0)
		{
			Write-Host "Row: $lineNumber - Item existed already ==> Updating..... " -ForegroundColor Yellow
			UpdateData $list  $itemResults[0].ID $itemData
		}
		else
		{
			Write-Host "Row: $lineNumber - Inserting... " -ForegroundColor Green
			InsertData $list $itemData 
		}
	}
	catch
    {
		"Error - ImportDataToList $list $itemData $lineNumber : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportDataToCalendar.txt" -Append;   
		Write-host "Error : $_" -ForegroundColor Red
	}
}

function InsertData($list, $itemData)
{
	try {
		$newCalendarItem = $list.Items.Add()
		$newCalendarItem["Start Time"] =  $itemData.StartDate + " " + $itemData.StartTime
		$newCalendarItem["End Time"] =  $itemData.EndDate + " " + $itemData.EndTime
		$newCalendarItem["Title"] = $itemData.Title
		$newCalendarItem["Category"] = $itemData.Category
		$newCalendarItem.Update()
		Write-Host "Inserted successfully" -ForegroundColor Green
	}
	catch
    {
		"Error - Insert Data $list $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportDataToCalendar.txt" -Append;   
		Write-host "Error : $_" -ForegroundColor Red
	}
}

function UpdateData($list, $itemId, $itemData)
{
	try 
	{
		$itemToUpdate = $list.GetItemById($itemId)
		$itemToUpdate["Title"] = $itemData.Title
		$itemToUpdate["Category"] = $itemData.Category
		$itemToUpdate.Update()
		Write-Host "Updated successfully" -ForegroundColor Green
	}
	catch
    {
		$currentDate = Get-Date
		"$currentDate : Error - UpdateData $list $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportDataToCalendar.txt" -Append;   
		Write-host "Error : $_" -ForegroundColor Red
	}
}

function IsValidDate([string]$date)
{
	$result = 0
	try 
	{
		if (!([DateTime]::TryParse($date, [ref]$result)))
		{
			 $result = 0
		 }
		else
		{
			$result = 1
		}
	}
	catch
    {
		$currentDate = Get-Date
		"$currentDate : Error - UpdateData $list $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportDataToCalendar.txt" -Append;   
		Write-host "Error : $_" -ForegroundColor Red
	}
   return $result
}

Start-Transcript  
Write-Host "START IMPORT DATA TO CALENDAR" -ForegroundColor Blue
Main $SiteURL 
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript

Remove-PsSnapin Microsoft.SharePoint.PowerShell