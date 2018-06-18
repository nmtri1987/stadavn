echo off
cd /d %~dp0
SET SiteURL="http://windev162:1111" 

::******************* IMPORT CONFIGURATION DATA **********************
powershell.exe -File ImportConfigurations.ps1 -SiteURL %SiteURL% 

pause