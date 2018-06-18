echo off
cd /d %~dp0
SET WebAppURL="http://localhost"
SET SiteURL="http://localhost"
:: Updating for Request and Recruitment modules
:: - Fix bug sending email for reject.
:: Update DLL: RBVH.Stada.Intranet.Biz
:: Run powershell script: ImportWorkflowEmailTemplate.ps1
:: Deploy Solutions
powershell.exe -File DeployPackages.ps1 -SiteURL %SiteURL%

powershell.exe -File ActiveFeatureForWebPages.ps1 -WebAppURL %WebAppURL% -SiteURL %SiteURL%

::******************* IMPORT MASTER DATA **********************
powershell.exe -File ImportWorkflowEmailTemplate.ps1 -SiteURL %SiteURL% 

pause