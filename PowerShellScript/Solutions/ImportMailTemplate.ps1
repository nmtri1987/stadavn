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
	$filePath = ".\CSVFile\MailTemplateData.csv"
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
		$mailTemplateList = $web.Lists["Mail Template"]

		foreach($item in Import-Csv $shiftTimeCsvFilePath)
		{
			$isExistedId = CheckIfKeyExist $mailTemplateList $item.Key
			
			if($isExistedId -eq 0)
			{
				Write-Host "Inserting ... " -ForegroundColor Green
				InsertMailTemplateData $mailTemplateList $item
			}
			else
			{
				Write-Host "Updating ..." -ForegroundColor Yellow
				UpdateMailTemplateData  $mailTemplateList $isExistedId $item
			}
		}
		$web.Dispose()
	}
}

function CheckIfKeyExist($list, $key)
{
	$qryCode = new-object Microsoft.SharePoint.SPQuery
	$qryCode.Query = "<Where><Eq> <FieldRef Name='MailKey' /><Value Type='Text'>"+$key+"</Value></Eq></Where>"
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

function InsertMailTemplateData($list, $itemData)
{
	try {
		$newItem = $list.Items.Add()
		$newItem["MailKey"] = $itemData.Key
		$newItem["MailSubject"] = $itemData.Subject
		$newItem["MailBody"] = $itemData.Body
		$newItem["MailCategory"] =  $itemData.Category
        $newItem["MailNote"] = $itemData.Note 
		$newItem.Update()
		Write-Host "Inserted successfully" -ForegroundColor Green
	}
	catch
    {
		$currentDate = Get-Date
		"$currentDate : Error - Insert Data $list $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportMailTemplate.txt" -Append;   
		Write-host "Error - Insert Data $list $itemData : $_"  -ForegroundColor Red
	}
}

function UpdateMailTemplateData($list, $itemId, $itemData)
{
	try 
	{
		$itemToUpdate = $list.GetItemById($itemId)
		$itemToUpdate["MailKey"] = $itemData.Key
		$itemToUpdate["MailSubject"] = $itemData.Subject
		$itemToUpdate["MailBody"] = $itemData.Body
		$itemToUpdate["MailCategory"] = $itemData.Category
        $itemToUpdate["MailNote"] = $itemData.Note 
		$itemToUpdate.Update()
		Write-Host "Updated successfully" -ForegroundColor Green
	}
	catch
    {
		$currentDate = Get-Date
		"$currentDate : Error - UpdateData $list $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportMailTemplate.txt" -Append;   
		Write-host "Error - UpdateData $list $itemData : $_" -ForegroundColor Red
	}
}

Start-Transcript  
Write-Host "START IMPORT DATA TO MAIL TEMPLATE" -ForegroundColor Blue
Main $SiteURL 
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript

Remove-PsSnapin Microsoft.SharePoint.PowerShell