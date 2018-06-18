echo off
cd /d %~dp0

::******************* UPDATE CONFIGURATION DATA **********************
powershell.exe -File UpdateCustomServices.ps1

pause