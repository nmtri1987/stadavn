
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
    ActiveFeatures_Web $SiteURL
}

function ActiveFeatures_Web($SiteURL)
{
    Write-Host ""
    Write-Host -Fore Yellow "Web Features:"
    Write-Host "==========================================="


    Write-Host -NoNewline  "Activating RBVH.Stada.Intranet.WebPages Feature ... "
    Enable-SPFeature –Identity "410c26d8-af97-4946-b3bf-6872e77465fd" –url $SiteURL -Force
    Write-Host -Fore Green "-> activated Successfully"
}

Start-Transcript

Header

Main $SiteURL

Footer

Stop-Transcript 