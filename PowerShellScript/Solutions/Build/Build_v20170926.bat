echo off
cd /d %~dp0
SET WebAppURL="http://localhost" 
SET SiteURL="http://localhost" 

::Deploy Solutions
powershell.exe -File InstallSolutions.ps1 -SiteURL %SiteURL%

::******************* IMPORT MASTER DATA **********************
powershell.exe -File ImportWorkflowEmailTemplate.ps1 -SiteURL %SiteURL% 
