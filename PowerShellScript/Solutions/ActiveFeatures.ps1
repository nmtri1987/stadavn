
Param($SiteURL);

if ($SiteURL -eq $null) { $SiteURL = "http://localhost"}


Add-PSSnapin Microsoft.Sharepoint.Powershell
set-executionpolicy remotesigned

function Header()
{
    Write-Host "-------------------------------------------"
    write-host -Fore Green "Start Setup Features"
    write-host -Fore Green "SiteUrl: " $SiteURL
    Write-Host "-------------------------------------------"
}
function Footer()
{
    Write-Host ""
    Write-Host "-------------------------------------------"
    Write-Host -Fore Green "End Setup Features"
    Write-Host "-------------------------------------------"
}

function Main($SiteURL)
{
    Write-Host -NoNewline  "Activating RBVH.Stada.Intranet.Resources Feature ... "
    Enable-SPFeature –Identity "1f0fdf7c-2663-4695-8df6-22bfe05aac80" –url $SiteURL -Force
    Write-Host -Fore Green "-> activated Successfully"

    ActiveFeatures_SiteCollection $SiteURL
    ActiveFeatures_Web $SiteURL
}

function ActiveFeatures_SiteCollection($SiteURL)
{
    Write-Host ""
    Write-Host -Fore Yellow "Site collection Features:"
    Write-Host "==========================================="
	
	try 
	{
		Write-Host -NoNewline "DeActivating Features: MDSFeature"
		Disable-SPFeature -Identity "MDSFeature" -Url $SiteURL -Force -Confirm:$false -ErrorAction SilentlyContinue
		Write-Host -Fore Green "-> deactivated Successfully"
	}
	catch
    {
		"Error - Error Disable-SPFeature MDSFeature Exception: $_.Exception.Message" |Out-File $currentDirectory"\PowerShellLogs\ActivateFeature.txt" -Append;   
		Write-Output "Error : $_.Exception.Message"
	}

	try 
	{
		Write-Host -NoNewline "Activating PublishingSite Feature ... "
		$fPublishingSite = Get-SPFeature PublishingSite
		Enable-SPFeature -Identity $fPublishingSite -Url $SiteURL -Force -ErrorAction SilentlyContinue
		Write-Host -Fore Green "-> activated Successfully"
	}
	catch
    {
		"Error - Error Activating PublishingSite Feature Exception: $_.Exception.Message" |Out-File $currentDirectory"\PowerShellLogs\ActivateFeature.txt" -Append;   
		Write-Output "Error : $_.Exception.Message"
	}

	try 
	{
		Write-Host -NoNewline "Activating PublishingWeb Feature ... "
		$fPublishingWeb = Get-SPFeature PublishingWeb
		Enable-SPFeature -Identity $fPublishingWeb -Url $SiteURL -Force -ErrorAction SilentlyContinue
		Write-Host -Fore Green "-> activated Successfully"
	
	}

	catch
    {
		"Error - Error Activating PublishingWeb Feature: $_.Exception.Message" |Out-File $currentDirectory"\PowerShellLogs\ActivateFeature.txt" -Append;   
		Write-Output "Error : $_.Exception.Message"
	}

	try 
	{
		Write-Host -NoNewline "Activating RBVH.Stada.Intranet.SiteColumns Feature ... "
		Enable-SPFeature –Identity "05b0392a-2a81-4e4c-a238-30e5223ce72e" –url $SiteURL -Force
		Write-Host -Fore Green "-> activated Successfully"
	}
	catch
    {
		"Error - Activating RBVH.Stada.Intranet.SiteColumns: $_.Exception.Message" |Out-File $currentDirectory"\PowerShellLogs\ActivateFeature.txt" -Append;   
		Write-Output "Error : $_.Exception.Message"
	}

	try 
	{
		Write-Host -NoNewline  "Activating RBVH.Stada.Intranet.ContentTypes Feature ... "
		Enable-SPFeature –Identity "2fb92f93-ceba-48f6-90e2-4077f302f67c" –url $SiteURL -Force
		Write-Host -Fore Green "-> activated Successfully"
	}
	catch
    {
		"Error - Activating RBVH.Stada.Intranet.ContentTypes: $_.Exception.Message" |Out-File $currentDirectory"\PowerShellLogs\ActivateFeature.txt" -Append;   
		Write-Output "Error : $_.Exception.Message"
	}

	try 
	{
		Write-Host ""
		Write-Host -NoNewline "Activating RBVH.Stada.Intranet.Branding Feature ... "
		Enable-SPFeature –Identity "c02e2723-eb5c-4877-93d6-690cc7e7751b" –url $SiteURL  -Force
		Write-Host -Fore Green "-> activated Successfully"
	}
	catch
    {
		"Error - Activating RBVH.Stada.Intranet.Branding: $_.Exception.Message" |Out-File $currentDirectory"\PowerShellLogs\ActivateFeature.txt" -Append;   
		Write-Output "Error : $_.Exception.Message"
	}
}

function ActiveFeatures_Web($SiteURL)
{
    Write-Host ""
    Write-Host -Fore Yellow "Web Features:"
    Write-Host "==========================================="

    Write-Host -NoNewline  "Activating RBVH.Stada.Intranet.ListDenifitions Feature ... "
    Enable-SPFeature –Identity "daa42646-bd4b-472c-9dae-5cbddfb09afb" –url $SiteURL  -Force #feature Web
    Enable-SPFeature –Identity "3f056ea5-397d-4aa8-a799-9bf2d4b51238" –url $SiteURL  -Force #document
	Enable-SPFeature –Identity "bb4867d6-8dc1-43de-9a8d-b643fd7784f4" –url $SiteURL  -Force #supporting documents
    Write-Host -Fore Green "-> activated Successfully"

    Write-Host -NoNewline  "Activating RBVH.Stada.Intranet.WebPages Feature ... "
    Enable-SPFeature –Identity "410c26d8-af97-4946-b3bf-6872e77465fd" –url $SiteURL -Force
    Write-Host -Fore Green "-> activated Successfully"

	#No need to active SiteTemplate feature

    Write-Host -NoNewline  "Activating RBVH.Stada.Intranet.Branding Feature ... "
    Enable-SPFeature –Identity "0449259b-f1d4-4a86-a202-35dd287c1d6e" –url $SiteURL -Force
    Write-Host -Fore Green "-> activated Successfully"

    
    Write-Host -NoNewline  "RBVH.Stada.Intranet.ListEventReceiver Feature ... "
    Enable-SPFeature –Identity "fa321729-2453-49f9-9965-3d52d3c0bdae" –url $SiteURL -Force
    Write-Host -Fore Green "-> activated Successfully"
}

Start-Transcript

Header

Main $SiteURL

Footer

Stop-Transcript 