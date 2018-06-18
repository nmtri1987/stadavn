echo off
cd /d %~dp0

SET SiteURL="http://tronghieusp" 

::Deploy Solutions
powershell.exe -File DeployRequestDiploma.ps1 -SiteURL %SiteURL%  

pause