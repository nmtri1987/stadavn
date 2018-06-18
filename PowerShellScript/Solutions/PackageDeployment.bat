echo off
cd /d %~dp0
SET WebAppURL="http://localhost" 
SET SiteURL="http://localhost" 

SET ResourcesPath="RBVH.Stada.Intranet.Resources.wsp"
SET BrandingPath="RBVH.Stada.Intranet.Branding.wsp"
SET SiteColumnsPath="RBVH.Stada.Intranet.SiteColumns.wsp"
SET ContentTypesPath="RBVH.Stada.Intranet.ContentTypes.wsp"
SET ListDefinitionsPath="RBVH.Stada.Intranet.ListDefinitions.wsp"
SET WebPagesPath="RBVH.Stada.Intranet.WebPages.wsp"
SET LeaveManagementPath="RBVH.Stada.Intranet.LeaveManagement.wsp"
SET ShiftManagementPath="RBVH.Stada.Intranet.ShiftManagement.wsp"
SET ChangeShiftManagementPath="RBVH.Stada.Intranet.ChangeShiftManagement.wsp"
SET ListEventReceiverPath="RBVH.Stada.Intranet.ListEventReceiver.wsp"

::Deploy Solutions
powershell.exe -File InstallSolutions.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%  -SolutionPath %ResourcesPath% -GacDep 0
powershell.exe -File InstallSolutions.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%  -SolutionPath %SiteColumnsPath% -GacDep 0 
powershell.exe -File InstallSolutions.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%  -SolutionPath %ContentTypesPath% -GacDep 0
powershell.exe -File InstallSolutions.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%  -SolutionPath %ListDefinitionsPath% -GacDep 0

powershell.exe -File InstallSolutions.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%  -SolutionPath %WebPagesPath% -GacDep 1
powershell.exe -File InstallSolutions.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%  -SolutionPath %BrandingPath% -GacDep 1

powershell.exe -File InstallSolutions.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%  -SolutionPath %LeaveManagementPath% -GacDep 0
powershell.exe -File InstallSolutions.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%  -SolutionPath %ShiftManagementPath% -GacDep 0
powershell.exe -File InstallSolutions.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%  -SolutionPath %ChangeShiftManagementPath% -GacDep 0
powershell.exe -File InstallSolutions.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%  -SolutionPath %ListEventReceiverPath% -GacDep 0

powershell.exe -File ActiveFeatures.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%

pause