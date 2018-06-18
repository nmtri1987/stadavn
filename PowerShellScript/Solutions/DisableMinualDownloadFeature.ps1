Param($SiteURL);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

function Main($SiteURL)
{
	if($SiteURL -eq $null)
	{
		$SiteURL ="http://localhost"
	}
	Disable-SPFeature -identity "87294C72-F260-42f3-A41B-981A2FFCE37A" -URL $SiteURL -Force -ErrorAction SilentlyContinue -Confirm:$false
}

Start-Transcript

Write-Host " "
Write-Host "START DISABLE MINUAL DOWNLOAD" -ForegroundColor BLUE
Main $SiteURL 

Write-Host "-- Done --"
Stop-Transcript