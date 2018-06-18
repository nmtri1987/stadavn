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
	$filePath = ".\CSVFile\MeetingRoomEquipments.csv"
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		$web = Get-SPWeb $SiteURL
		$meetingRoomEquipmentListCSVFilePath = get-item $filePath
		Write-Host "Path of file :: " $meetingRoomEquipmentListCSVFilePath
		Write-Host "Reading " $meetingRoomEquipmentListCSVFilePath " file" -ForegroundColor Green
		$meetingRoomEquipmentList = $web.Lists["Equipments"]
		
		foreach($item in Import-Csv $meetingRoomEquipmentListCSVFilePath)
		{
			InsertListItem $meetingRoomEquipmentList $item
		}
		$web.Dispose()
	}
}

function InsertListItem($list, $itemData)
{
	try 
	{
		$queryMeetingRoomEquipment = New-Object  Microsoft.SharePoint.SPQuery 
		$queryMeetingRoomEquipment.query ="<Where><Eq><FieldRef Name='CommonName' /><Value Type='Text'>" + $itemData.NameEN +"</Value></Eq></Where>"
		$items = $list.getItems($queryMeetingRoomEquipment)
		if ($items.Count -gt 0)
		{
			$item = $items[0]
			$item["CommonName1066"] = $itemData.NameVN
			$item.Update()
			Write-Host $items[0]["CommonName"] "is updated." -ForegroundColor Green
		}
		else
		{
			$newItem = $list.Items.Add()
			$newItem["Title"] = $itemData.Title
			$newItem["CommonName"] = $itemData.NameEN
			$newItem["CommonName1066"] = $itemData.NameVN
			$newItem.Update()
			Write-Host $itemData.Title "is added." -ForegroundColor Green
		}
	}
	catch
    {
		$currentDate = Get-Date
		"$currentDate : Error - Insert Data $list $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportMeetingRoomEquipments.txt" -Append;   
		Write-host "Error - Insert Data $list $itemData : $_"  -ForegroundColor Red
	}
}

Start-Transcript  
Write-Host "START IMPORT DATA TO MEETING ROOMS LIST" -ForegroundColor Blue
Main $SiteURL 
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript

Remove-PsSnapin Microsoft.SharePoint.PowerShell