echo off
cd /d %~dp0
SET WebAppURL="http://localhost" 
SET SiteURL="http://localhost" 
SET WebAppFolder="portal.stadavn.int80"
::Deploy Solutions
powershell.exe -File InstallSolutions.ps1 -SiteURL %SiteURL%  

powershell.exe -File DisableMinualDownloadFeature.ps1 -SiteURL %SiteURL%  

::Copy Resources
powershell.exe -File CopyResource.ps1 -SiteURL %SiteURL% -WebAppFolder %WebAppFolder%

powershell.exe -File ActiveFeatures.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%

powershell.exe -File CreatePoliciesSite.ps1 -SiteURL %SiteURL%


::Import/Update Factory Location
powershell.exe -File ImportFactoryLocation.ps1 -SiteURL %SiteURL% 

::Important => need import department first, so that subsite can be created
powershell.exe -File ImportDepartment.ps1 -SiteURL %SiteURL%

:: waiting creating new department site
pause

::Create groups: BOD, Common Accounts, System Admin
powershell.exe -File CreateGroup.ps1 -SiteURL %SiteURL%

::******************* ASSIGN PERMISSION **********************
powershell.exe -File AssignReadPermissionForAllLists.ps1 -SiteURL %SiteURL%
::Assign Read Permission  for  BOD, Common Accounts, System Admin
powershell.exe -File AssignReadPermissionRootSiteInMainGroup.ps1 -SiteURL %SiteURL%
::Assign CONTRIBUTE Permission  for all Lists
powershell.exe -File AssignContributorPermissionForAllLists.ps1 -SiteURL %SiteURL%
::Assign CONTRIBUTE Permission  by MODULE
powershell.exe -File AssignContributorPermissionByModule.ps1 -SiteURL %SiteURL%

::******************* IMPORT MASTER DATA **********************
::IMPORT CALENDAR
powershell.exe -File ImportDataToCalendar.ps1 -SiteURL %SiteURL%
::IMPORT SHIFTTIME DATA
powershell.exe -File ImportShiftTimeData.ps1 -SiteURL %SiteURL%
::Import/Update Employee Position
powershell.exe -File ImportEmployeePosition.ps1 -SiteURL %SiteURL% 
::Import/Update Company Vehicle
powershell.exe -File ImportCompanyVehicles.ps1 -SiteURL %SiteURL% 
:: Import group to Group List
powershell.exe -File ImportGroups.ps1 -SiteURL %SiteURL% 
::Import work flow step list
powershell.exe -File ImportWorkflowStepList.ps1 -SiteURL %SiteURL% 

powershell.exe -File ImportRequestType.ps1 -SiteURL %SiteURL% 

powershell.exe -File ImportWorkflowEmailTemplate.ps1 -SiteURL %SiteURL% 

::Import Permission Page
powershell.exe -File ImportPermissionPage.ps1 -SiteURL %SiteURL% 
::Import mail teamplate
powershell.exe -File ImportMailTemplate.ps1 -SiteURL %SiteURL%
::Import Step List
powershell.exe -File ImportStepList.ps1 -SiteURL %SiteURL%

::Enable version history
powershell.exe -File EnableVersionHistoryForList.ps1 -SiteURL %SiteURL% 
::Set language
powershell.exe -File SetLanguageForCreatedSite.ps1 -SiteURL %SiteURL%

::Update BOD for departments
powershell.exe -File UpdateDepartmentBOD.ps1 -SiteURL %SiteURL%

::Import language level
powershell.exe -File ImportForeignLanguageLevel.ps1 -SiteURL %SiteURL%
::Import language
powershell.exe -File ImportForeignLanguage.ps1 -SiteURL %SiteURL%

::******************* IMPORT EMPLOYEE **********************
SET SPUserFilePath=".\CSVFile\STADA-SPUsers.csv"
SET CommonUserFilePath=".\CSVFile\STADA-CommonUsers.csv"
SET TestUserFilePath=".\CSVFile\STADA-TestUsers.csv"
SET TestAdminUserFilePath=".\CSVFile\STADA-AdminUsers.csv"
SET AllUsersFilePath=".\CSVFile\STADA-AllUsersUpdate.csv"
SET RecruitmentTeamFilePath=".\CSVFile\RecruitmentTeam.csv"

::powershell.exe -File ImportEmployee.ps1 -SiteURL %SiteURL% -FilePath %SPUserFilePath%
::powershell.exe -File ImportEmployee.ps1 -SiteURL %SiteURL% -FilePath %CommonUserFilePath%
::powershell.exe -File ImportEmployee.ps1 -SiteURL %SiteURL% -FilePath %TestUserFilePath%
::powershell.exe -File ImportEmployee.ps1 -SiteURL %SiteURL% -FilePath %TestAdminUserFilePath%
powershell.exe -File ImportEmployee.ps1 -SiteURL %SiteURL% -FilePath %AllUsersFilePath%
powershell.exe -File ImportRecruitmentTeam.ps1 -SiteURL %SiteURL% -FilePath %RecruitmentTeamFilePath%

::Import AdditionalEmployeePosition
powershell.exe -File ImportAdditionalEmployeePosition.ps1 -SiteURL %SiteURL%
::Import Freight Receiver Department
powershell.exe -File ImportFreightReceiverDepartment.ps1 -SiteURL %SiteURL%
::Import Freight Vehicle
powershell.exe -File ImportFreightVehicles.ps1 -SiteURL %SiteURL%
::Import Import Delegation Module
powershell.exe -File ImportDelegationModule.ps1 -SiteURL %SiteURL%
::Import Import Delegation Employee Position
powershell.exe -File ImportDelegationEmployeePositions.ps1 -SiteURL %SiteURL%


::******************* IMPORT CONFIGURATION DATA **********************
powershell.exe -File ImportConfigurations.ps1 -SiteURL %SiteURL% 


::Create Calendar overlay
powershell.exe -File CalendarOverlayMainSite.ps1 -SiteURL %SiteURL% 
powershell.exe -File CalendarOverlaySubsite.ps1 -SiteURL %SiteURL% 
powershell.exe -File AddMiniCalendarWebPartMainSite.ps1 -SiteURL %SiteURL% 
powershell.exe -File AddMiniCalendarSubSite.ps1 -SiteURL %SiteURL% 

:: upload Files to Document List
powershell.exe -File UploadFileToDocumentList.ps1 -SiteURL %SiteURL%
powershell.exe -File UploadEmployeePictureToImagesList.ps1 -SiteURL %SiteURL%

powershell.exe -File CreateFolderInFormList.ps1 -SiteURL %SiteURL%

pause