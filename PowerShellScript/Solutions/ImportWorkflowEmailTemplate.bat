echo off
cd /d %~dp0
SET SiteURL="http://localhost:1111" 

::******************* IMPORT MASTER DATA **********************
powershell.exe -File ImportWorkflowEmailTemplate.ps1 -SiteURL %SiteURL% 

pause