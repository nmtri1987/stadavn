echo off
cd /d %~dp0
SET WebAppURL="http://localhost" 
SET SiteURL="http://localhost" 

::Active Features
powershell.exe -File ActiveFeatures.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%

pause