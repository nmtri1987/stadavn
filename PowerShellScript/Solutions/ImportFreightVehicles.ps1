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
		$FilePath = ".\CSVFile\FreightVehicles.csv"
	}
		
	Write-Host $SiteURL
		
	$web = Get-SPWeb $SiteURL
		
	$freightVehicleList = $web.Lists["Freight Vehicle"]
		
	Write-Host "Group" $freightVehicleList 
		
	$source = Import-Csv -Delimiter "," -Path $FilePath
   
		foreach ($item in $source)
		{
			try{
				$currentItem = CheckIfItemExist $freightVehicleList $item
				if($currentItem -ne $null)
				{
					SaveItem $currentItem $item
				}
				else
				{
					$currentItem = $freightVehicleList.Items.Add()
					SaveItem $currentItem $item
				}
			}
			catch
			{
				$currentDate = Get-Date
				"$currentDate : Error - Import $item : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportFreightVehicle.txt" -Append;   
				Write-Host "Error : " $_ -ForegroundColor Red
			}
		}

	$web.Dispose()
}

function CheckIfItemExist($list, $itemdata)
{
	$qryCode = new-object Microsoft.SharePoint.SPQuery
	$qryCode.Query = "<Where><Eq> <FieldRef Name='Vehicle' /><Value Type='Text'>"+$itemdata.Vehicle+"</Value></Eq></Where>"
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
		Write-Host $itemdata.Vehicle $itemdata.VehicleVN
		$newitem["Vehicle"]= $itemdata.Vehicle
		$newitem["VehicleVN"]= $itemdata.VehicleVN
		$newitem.Update()
		Write-Host "Updated successfully" -ForegroundColor Green
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Import $itemdata : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportFreightVehicle.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}

Start-Transcript  
Write-Host "START IMRPORT FREIGHT VEHICLES DATA" -ForegroundColor Blue
Main $SiteURL $FilePath

Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell
