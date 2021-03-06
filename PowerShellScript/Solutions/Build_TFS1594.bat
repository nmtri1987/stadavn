echo off
cd /d %~dp0
SET WebAppURL="http://localhost" 
SET SiteURL="http://localhost" 
SET Port=80

::Deploy Solutions
powershell.exe -File InstallSolutions.ps1 -SiteURL %SiteURL%  

:: Copy Resources
powershell.exe -File CopyResource.ps1 -SiteURL %SiteURL% -Port %Port%

:: Reset IIS
powershell.exe -File iisreset.ps1

::Active Features
powershell.exe -File ActiveFeatures.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%

::******************* ASSIGN PERMISSION **********************
powershell.exe -File AssignReadPermissionForAllLists.ps1 -SiteURL %SiteURL%
::Assign CONTRIBUTE Permission  for all Lists
powershell.exe -File AssignContributorPermissionForAllLists.ps1 -SiteURL %SiteURL%

::******************* IMPORT MASTER DATA **********************
powershell.exe -File UpdateConfigurations.ps1 -SiteURL %SiteURL% 

pause