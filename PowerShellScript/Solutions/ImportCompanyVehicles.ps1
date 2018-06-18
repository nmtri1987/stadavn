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
		$FilePath = ".\CSVFile\CompanyVehicles.csv"
	}
		
	Write-Host $SiteURL
		
	$web = Get-SPWeb $SiteURL
		
	$CompanyVehiclesList = $web.Lists["Company Vehicles"]
		
	Write-Host "Group" $CompanyVehiclesList 
		
	$source = Import-Csv -Delimiter "," -Path $FilePath
   
		foreach ($item in $source)
		{
			try{
				Write-Host $item.Place $item.Time $item.Description 
				$newItem = $CompanyVehiclesList.Items.Add()
				$newitem["StadaDescription"] = $item.Description
				$newitem["Place"]= $item.Place
				$newitem["Time"]= $item.Time
				$newitem.Update()
			}
			catch
			{
				$currentDate = Get-Date
				"$currentDate : Error - Import $item : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportCompanyVehicles.txt" -Append;   
				Write-Host "Error : " $_ -ForegroundColor Red
			}
		}

	$CompanyVehiclesList.Update()
	#$rootWeb.Dispose()
	$web.Dispose()
	#$site.Dispose()
}


Start-Transcript  
Write-Host "START IMRPORT VEHICLES DATA" -ForegroundColor Blue
Main $SiteURL $FilePath

Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell
