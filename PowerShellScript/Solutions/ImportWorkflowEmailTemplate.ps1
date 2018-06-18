Param($SiteURL,$FilePath);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

if($SiteURL -eq $null){
	$SiteURL="http://localhost"
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main($SiteURL,$FilePath){

	if($FilePath -eq $null)
	{
		$FilePath = $currentDirectory + "\CSVFile\WorkflowEmailTemplateData.csv"
	}
		
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		Write-Host $SiteURL -ForegroundColor DarkBlue
		$web = Get-SPWeb $SiteURL
		$wfEmailCsvFilePath = get-item $filePath
		Write-Host "Path of file :: " $wfEmailCsvFilePath
		Write-Host "Reading " $wfEmailCsvFilePath " file" -ForegroundColor Green
		$wfEmailTemplateList = $web.Lists["Workflow email template"]
		foreach($item in Import-Csv $wfEmailCsvFilePath)
		{
			$isExistedId = CheckIfKeyExist $wfEmailTemplateList $item.Key
			Write-Host $item.Key
			if($isExistedId -ne 0)
			{
				Write-Host "Update WF email template: " $item.Key
				UpdateWFEmailTemplate $web $wfEmailTemplateList $isExistedId $item
			}
			else
			{
				InsertWFEmailTemplate $web $wfEmailTemplateList $item
	
			}						
		}
		$web.Dispose()
	}
}

function CheckIfKeyExist($list, $key)
{
	$qryCode = new-object Microsoft.SharePoint.SPQuery
	$qryCode.Query = "<Where><Eq> <FieldRef Name='Key' /><Value Type='Text'>"+$key+"</Value></Eq></Where>"
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

function InsertWFEmailTemplate($web,$list, $itemData)
{
	try{
		$newItem = $list.Items.Add()
		$newItem["Key"] = $itemData.Key
		$newItem["Subject"] =  $itemData.Subject
		$newItem["Body"] =  GetDataHtml $itemData.Body
		$newItem["ListName"] =  $itemData.ListName
		$newItem["Action"] = $itemData.Action 	
		$newItem.Update()
		Write-Host "Inserted successfully" -ForegroundColor Green
		}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Insert  WF email teamplate $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportWorkflowEmailTemplate.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}


function UpdateWFEmailTemplate($web, $list, $itemId, $itemData)
{
	try{

		$itemToUpdate = $list.GetItemById($itemId)

		$itemToUpdate["Key"] = $itemData.Key
		$itemToUpdate["Subject"] = $itemData.Subject
		$itemToUpdate["Body"] = GetDataHtml $itemData.Body
		$itemToUpdate["ListName"] =  $itemData.ListName
		$itemToUpdate["Action"] = $itemData.Action 	

		$itemToUpdate.Update()
		Write-Host "Updated successfully" -ForegroundColor Green
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Update WF email teamplate $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportWorkflowEmailTemplate.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}

function GetDataHtml($htmlString)
{
	if($htmlString -ne $null -AND $htmlString -ne "")
	{
		$html = $htmlString -replace "{}",'"'
		return $html
	}
	else
	{
		return ""
	}
}


Start-Transcript  
Write-Host "START IMRPORT WF EMAIL TEMPLATE DATA" -ForegroundColor Blue
Main $SiteURL $FilePath
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell