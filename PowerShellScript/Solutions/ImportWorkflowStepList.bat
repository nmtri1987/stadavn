echo off
cd /d %~dp0
SET WebAppURL="http://localhost" 
SET SiteURL="http://localhost" 

::******************* IMPORT MASTER DATA **********************
::Import work flow step list
powershell.exe -File ImportWorkflowStepList.ps1 -SiteURL %SiteURL%

pause