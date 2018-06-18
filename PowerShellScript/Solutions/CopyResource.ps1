Param($SiteURL,$WebAppFolder);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
            Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

Function CopyResource($SiteURL, $WebAppFolder)
{
	Write-Host "Copying resource ... "
	Copy-Item ("C:\inetpub\wwwroot\wss\VirtualDirectories\" + $WebAppFolder + "\App_GlobalResources\RBVHStadaLists.resx") ("C:\inetpub\wwwroot\wss\VirtualDirectories\" + $WebAppFolder + "\App_GlobalResources\RBVHStadaLists.en-US.resx") 
	Copy-Item ("C:\inetpub\wwwroot\wss\VirtualDirectories\" + $WebAppFolder + "\App_GlobalResources\RBVHStadaWebpages.resx") ("C:\inetpub\wwwroot\wss\VirtualDirectories\" + $WebAppFolder + "\App_GlobalResources\RBVHStadaWebpages.en-US.resx") 

	Copy-Item "C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\16\Resources\RBVHStadaLists.resx" "C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\16\Resources\RBVHStadaLists.en-US.resx"
	Copy-Item "C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\16\Resources\RBVHStadaWebpages.resx" "C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\16\Resources\RBVHStadaWebpages.en-US.resx"
	Write-Host "CopyResource Is DONE"
}

iisreset

Start-Transcript
Write-Host "STARTING..." -ForegroundColor Blue
CopyResource $SiteURL $WebAppFolder
Write-Host "DONE" -ForegroundColor Blue
Stop-Transcript