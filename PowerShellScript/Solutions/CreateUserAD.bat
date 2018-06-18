echo off
cd /d %~dp0
SET WebAppURL="http://windev162:1111" 
SET SiteURL="http://windev162:1111" 

SET FilePath="CSVFile/STADA-AllUsersUpdate.csv"

::Create User AD
powershell.exe -File CreateUserAD.ps1 -SiteURL %SiteURL% -FilePath %FilePath%

pause