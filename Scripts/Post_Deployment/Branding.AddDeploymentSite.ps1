Param($SiteURL, $DirPath);

Write-Host $SiteURL
Write-Host $DirPath

Add-PsSnapin Microsoft.SharePoint.PowerShell
#set-executionpolicy remotesigned

## SharePoint DLL 
[void][System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SharePoint") 
Write-Output " "
Write-Output "Checking for policies template..."
#$PoliciesDirPath=$DirPath+"\PoliciesSite.wsp"

#Write-Output "Uploading New Template..."  
#Add-SPUserSolution -LiteralPath $PoliciesDirPath -Site $SiteURL
#Install-SPUserSolution -Identity "PoliciesSite.wsp" -Site $SiteURL

$DepartmentDirPath=$DirPath+"\Department Site.wsp"

Write-Output "Uploading New Template..."  
Add-SPUserSolution -LiteralPath $DepartmentDirPath -Site $SiteURL
Install-SPUserSolution -Identity "Department Site.wsp" -Site $SiteURL