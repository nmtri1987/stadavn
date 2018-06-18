echo off
cd /d %~dp0
SET WebAppURL="http://localhost"
SET SiteURL="http://localhost"
SET WebAppFolder=80

::Copy Resources
powershell.exe -File CopyResource.ps1 -SiteURL %SiteURL% -WebAppFolder %WebAppFolder%
iisreset

pause