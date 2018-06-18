echo off
cd /d %~dp0
SET SiteURL="http://localhost" 

::******************* UPDATE CONFIGURATION DATA **********************
powershell.exe -File UpdateConfigurations.ps1 -SiteURL %SiteURL% 

pause