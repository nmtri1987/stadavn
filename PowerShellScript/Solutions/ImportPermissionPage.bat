echo off
cd /d %~dp0
SET SiteURL="http://localhost"

::Import Permission Page
powershell.exe -File ImportPermissionPage.ps1 -SiteURL %SiteURL%

pause