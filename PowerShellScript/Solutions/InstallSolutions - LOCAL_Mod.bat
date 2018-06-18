echo off
cd /d %~dp0
SET WebAppURL="http://localhost" 
SET SiteURL="http://localhost" 
SET WebAppFolder="80"
::Deploy Solutions

::Deploy Solutions
powershell.exe -File InstallSolutions.ps1 -SiteURL %SiteURL%  

::Active Features
powershell.exe -File ActiveFeatures.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%


pause