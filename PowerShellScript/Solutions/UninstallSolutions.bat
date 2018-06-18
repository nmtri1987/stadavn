echo off
cd /d %~dp0
SET WebAppURL="http://localhost" 


::UninstallSolutions
powershell.exe -File UninstallSolutions.ps1  -WebAppURL %WebAppURL% 
pause