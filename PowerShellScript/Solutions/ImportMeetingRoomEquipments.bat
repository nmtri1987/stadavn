echo off
cd /d %~dp0
SET SiteURL="http://windev1442"

::Import Meeting Room Equipments
powershell.exe -File ImportMeetingRoomEquipments.ps1 -SiteURL %SiteURL%

pause