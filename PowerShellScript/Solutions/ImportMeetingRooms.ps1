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
	$filePath = ".\CSVFile\MeetingRooms.csv"
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		$web = Get-SPWeb $SiteURL
		$meetingRoomListCSVFilePath = get-item $filePath
		Write-Host "Path of file :: " $meetingRoomListCSVFilePath
		Write-Host "Reading " $meetingRoomListCSVFilePath " file" -ForegroundColor Green
		$meetingRoomList = $web.Lists["Meeting rooms"]
		
		foreach($item in Import-Csv $meetingRoomListCSVFilePath)
		{
			InsertListItem $meetingRoomList $item
		}
		$web.Dispose()
	}
}

function InsertListItem($list, $itemData)
{
	try 
	{
		$queryMeetingRoom = New-Object  Microsoft.SharePoint.SPQuery 
		$queryMeetingRoom.query ="<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + $itemData.Title +"</Value></Eq></Where>"
		$items = $list.getItems($queryMeetingRoom)
		if ($items.Count -gt 0)
		{
			Write-Host $items[0].Title "is existed." -ForegroundColor Green
		}
		else
		{
			$newItem = $list.Items.Add()
			$newItem["Title"] = $itemData.Title
			$newItem.Update()
			Write-Host $itemData.Title "is added." -ForegroundColor Green
		}
	}
	catch
    {
		$currentDate = Get-Date
		"$currentDate : Error - Insert Data $list $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportMeetingRooms.txt" -Append;   
		Write-host "Error - Insert Data $list $itemData : $_"  -ForegroundColor Red
	}
}

Start-Transcript  
Write-Host "START IMPORT DATA TO MEETING ROOMS LIST" -ForegroundColor Blue
Main $SiteURL 
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript

Remove-PsSnapin Microsoft.SharePoint.PowerShell