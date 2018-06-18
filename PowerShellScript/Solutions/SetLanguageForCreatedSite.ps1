Param($SiteURL);

Add-PsSnapin Microsoft.SharePoint.PowerShell
set-executionpolicy remotesigned

## SharePoint DLL 
[void][System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SharePoint") 

#Set installed language activate for created site
Write-Output " "
Write-Host "START SET INSTALLED LANGUAGE ACTIVE FOR CREATED SITE" -ForegroundColor Blue

Write-Host $SiteURL -ForegroundColor DarkGreen
$SolutionURLForUploadTemplate = $SiteURL #Forexample:  http://sp-devbox2013
$SiteCollectionURL = $SiteURL #Forexample:  http://sp-devbox2013

$site = new-Object Microsoft.SharePoint.SPSite($SiteURL)
$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

foreach ($spWeb in $site.AllWebs)
{
	try{
		$spWeb.IsMultilingual = $true
		$WebRegionSettings = New-Object Microsoft.SharePoint.SPRegionalSettings($spWeb)
		foreach ($language in $WebRegionSettings.InstalledLanguages)
		{
			If ($language.DisplayName -eq "English" -or $language.DisplayName -eq "Vietnamese")
			# Add the displayname of any langauge you have installed: -or $language.DisplayName -eq "Norwegian" -or $language.DisplayName -eq "Finnish" -or $language.DisplayName -eq "Danish"
			{
				try{
					write-host -BackgroundColor Green -ForegroundColor Black "Update -" $spWeb "site with LCID:" $language.DisplayName
					$culture = New-Object System.Globalization.CultureInfo($language.LCID)
					$spWeb.AddSupportedUICulture($Culture)
				}
				catch{
					$currentDate = Get-Date
					"$currentDate : Error - Add Supported UI Culture $_" |Out-File $currentDirectory"\PowerShellLogs\SetLanguageForCreatedSite.txt" -Append;   
					Write-host " Error - Add Supported UI Culture $_" -ForegroundColor Red
				}
			}
			else
			{
				Write-host " Language not activated: " $language.DisplayName " on site " $spWeb.Name
			}
		}
		$spWeb.Update()
	}
	catch
    {
		$currentDate = Get-Date
		"$currentDate : Error - Set language $_" |Out-File $currentDirectory"\PowerShellLogs\SetLanguageForCreatedSite.txt" -Append;   
		Write-host " Error - Set language $_" -ForegroundColor Red
	}
}

Write-host "--DONE--" -ForegroundColor Blue

Remove-PsSnapin Microsoft.SharePoint.PowerShell