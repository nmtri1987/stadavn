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
		$FilePath = $currentDirectory + "\CSVFile\DelegationModules.csv"
	}
		
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		Write-Host $SiteURL -ForegroundColor DarkBlue
		$web = Get-SPWeb $SiteURL
		$delegationModuleCsvFilePath = get-item $filePath
		Write-Host "Path of file :: " $delegationModuleCsvFilePath
		Write-Host "Reading " $delegationModuleCsvFilePath " file" -ForegroundColor Green

		$delegationModuleList = $web.Lists["Delegations module"]
		foreach($item in Import-Csv $delegationModuleCsvFilePath)
		{
			$isExistedId = CheckIfTitleExist $delegationModuleList $item.Title
			Write-Host $item.Code
			if($isExistedId -ne 0)
			{
				Write-Host "Update department: " $item.ModuleName
				UpdateDelegationsModule $web $delegationModuleList $isExistedId $item
			}
			else
			{
				InsertDelegationsModule $web $delegationModuleList $item
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

function UpdateDelegationsModule($web, $list, $itemId, $itemData)
{
	try{
		$itemToUpdate = $list.GetItemById($itemId)

		$itemToUpdate["Title"] = $itemData.Title
		$itemToUpdate["ModuleName"] = $itemData.ModuleName
		$itemToUpdate["VietnameseModuleName"] = $itemData.VietnameseModuleName
		$itemToUpdate["ListURL"] = $itemData.ListURL

		$itemToUpdate.Update()
		Write-Host "Updated successfully" -ForegroundColor Green
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Update department $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportDelegationModule.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}

function InsertDelegationsModule($web,$list, $itemData)
{
	try{
		$newItem = $list.Items.Add()

		$newItem["Title"] = $itemData.Title
		$newItem["ModuleName"] = $itemData.ModuleName
		$newItem["VietnameseModuleName"] = $itemData.VietnameseModuleName
		$newItem["ListURL"] = $itemData.ListURL
		
		$newItem.Update()
		Write-Host "Inserted successfully" -ForegroundColor Green
		}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Insert delegation module $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportDelegationModule.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}

Start-Transcript  
Write-Host "START IMRPORT DELEGATION MODULE DATA" -ForegroundColor Blue
Main $SiteURL $FilePath
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell