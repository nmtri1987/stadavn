echo off
cd /d %~dp0
SET SiteURL="http://windev1442"

::Import Meeting Rooms
powershell.exe -File ImportMeetingRooms.ps1 -SiteURL %SiteURL%

pause