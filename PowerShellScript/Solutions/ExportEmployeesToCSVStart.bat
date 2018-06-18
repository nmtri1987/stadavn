echo off
cd /d %~dp0

SET SiteURL="http://windev16"
SET ADServer="SPLOCAL"

::Create User AD
powershell.exe -File ExportEmployeesToCSV.ps1 -ADServer %ADServer% -SiteURL %SiteURL%

pause