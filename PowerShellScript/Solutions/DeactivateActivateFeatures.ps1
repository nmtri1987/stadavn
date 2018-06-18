Param($SiteURL);
Write-Host $SiteURL -ForegroundColor DarkGreen



Add-Type -Path "RBVH.Stada.Intranet.Branding.dll"  

Add-PsSnapin Microsoft.SharePoint.PowerShell
set-executionpolicy remotesigned

function Main()
{
	$site = Get-SPSite $SiteURL
	#$web = Get-SPWeb $SiteURL
	
	Disable-SPFeature �Identity "c02e2723-eb5c-4877-93d6-690cc7e7751b" �url $SiteURL -Force -Confirm:$false  #branding scope site
	Enable-SPFeature �Identity "c02e2723-eb5c-4877-93d6-690cc7e7751b" �url $SiteURL   #branding scope site
	#ActiveFeture $site "c02e2723-eb5c-4877-93d6-690cc7e7751b"

	#branding scope web
	
	Disable-SPFeature �Identity "0449259b-f1d4-4a86-a202-35dd287c1d6e" �url $SiteURL -Force -Confirm:$false   #branding
	Enable-SPFeature �Identity "0449259b-f1d4-4a86-a202-35dd287c1d6e" �url $SiteURL    #branding 
	#ActiveFeture $web "0449259b-f1d4-4a86-a202-35dd287c1d6e"
	#$web.Dispose()
	$site.Dispose()
}

function ActiveFeture($site, $featureId)
{
	if(-not $site.Features[$featureId])
	{
		$site.Features.Add($featureId);
	}	
}








Start-Transcript  
Write-Host "TRY DEACTIVATE & ACTIVATE FEATURES" -ForegroundColor Blue
Main $SiteURL
Write-Host "DONE" -ForegroundColor Blue
Stop-Transcript

Remove-PsSnapin Microsoft.SharePoint.PowerShell