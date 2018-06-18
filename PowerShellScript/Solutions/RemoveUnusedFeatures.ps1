Param($SiteURL);

if ($SiteURL -eq $null) { $SiteURL = "http://localhost"}

Add-PSSnapin Microsoft.Sharepoint.Powershell
set-executionpolicy remotesigned


function Main($SiteURL)
{
   Uninstall-Features $SiteURL
    
}

function Uninstall-Features($SiteURL)
{
	Uninstall-SPFeature –Identity "5277a1a4-8b43-4c02-8322-50b887c9865b"  -Force   -ErrorAction SilentlyContinue -Confirm:$false  #change shift WF 

	Uninstall-SPFeature –Identity "17dfa2c0-3441-4feb-8ee4-f81b7bd3f846"   -Force  -ErrorAction SilentlyContinue  -Confirm:$false #leave WF
	Uninstall-SPFeature –Identity "5e5c8f35-6d29-49d9-9ba4-429e08deb666"   -Force -ErrorAction SilentlyContinue -Confirm:$false  #overtime WF
	#Uninstall-SPFeature –Identity "cc078e65-af28-4783-ae60-d8984593c979"   -Force -ErrorAction SilentlyContinue  -Confirm:$false  #notovertime WF

	Uninstall-SPFeature –Identity "41b99bc2-fafd-4c62-a83f-175b9c3dca94"   -Force -ErrorAction SilentlyContinue  -Confirm:$false  ##leave management
	Uninstall-SPFeature –Identity "6ebe7dc2-d361-470c-8114-3a0489584bdc"   -Force -ErrorAction SilentlyContinue  -Confirm:$false  ##shift management
}

Start-Transcript



Main $SiteURL



Stop-Transcript 