Param($SiteURL,$FilePath);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main($SiteURL,$FilePath){
	if($SiteURL -eq $null){
		$SiteURL="http://localhost"
	}
	if($FilePath -eq $null)
	{
		$FilePath = ".\CSVFile\FreightReceiverDepartment.csv"
	}
		
	Write-Host $SiteURL
		
	$web = Get-SPWeb $SiteURL
		
	$freightReceiverDeptList = $web.Lists["Freight Receiver Department"]
		
	Write-Host "Group" $freightReceiverDeptList 
		
	$source = Import-Csv -Delimiter "," -Path $FilePath
   
		foreach ($item in $source)
		{
			try{
				$currentItem = CheckIfItemExist $freightReceiverDeptList $item
				if($currentItem -ne $null)
				{
					SaveItem $currentItem $item
				}
				else
				{
					$currentItem = $freightReceiverDeptList.Items.Add()
					SaveItem $currentItem $item
				}
			}
			catch
			{
				$currentDate = Get-Date
				"$currentDate : Error - Import $item : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportFreightReceiverDepartment.txt" -Append;   
				Write-Host "Error : " $_ -ForegroundColor Red
			}
		}

	$web.Dispose()
}

function CheckIfItemExist($list, $itemdata)
{
	$qryCode = new-object Microsoft.SharePoint.SPQuery
	$qryCode.Query = "<Where><Eq> <FieldRef Name='ReceiverDepartment' /><Value Type='Text'>"+$itemdata.ReceiverDepartment+"</Value></Eq></Where>"
	$dataResults = $list.getItems($qryCode)
	if($dataResults.Count -eq 0)
	{
		return $null
	}
	else
	{
		return $dataResults[0]
	}
}

function SaveItem($newItem,$itemdata)
{
	try{
		Write-Host $itemdata.ReceiverDepartment $itemdata.ReceiverDepartmentVN
		$newitem["ReceiverDepartment"]= $itemdata.ReceiverDepartment
		$newitem["ReceiverDepartmentVN"]= $itemdata.ReceiverDepartmentVN
		$newitem.Update()
		Write-Host "Updated successfully" -ForegroundColor Green
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Import $itemdata : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportFreightReceiverDepartment.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}

Start-Transcript  
Write-Host "START IMRPORT FREIGHT RECEIVER DEPARTMENT DATA" -ForegroundColor Blue
Main $SiteURL $FilePath

Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell
