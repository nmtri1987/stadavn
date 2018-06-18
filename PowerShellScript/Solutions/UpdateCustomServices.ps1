if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null) 
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

Write-Host "Start Update Custom Services... " -ForegroundColor Green

$contentService = [Microsoft.SharePoint.Administration.SPWebService]::ContentService;

$wcfServiceSettings = New-Object Microsoft.SharePoint.Administration.SPWcfServiceSettings
$wcfServiceSettings.ReaderQuotasMaxStringContentLength = 2147483647
$wcfServiceSettings.ReaderQuotasMaxArrayLength = 2147483647
$wcfServiceSettings.ReaderQuotasMaxBytesPerRead = 2147483647
$wcfServiceSettings.MaxReceivedMessageSize = 2147483647
$wcfServiceSettings.MaxBufferSize = 2147483647
$wcfServiceSettings.ReaderQuotasMaxDepth = 2147483647
$wcfServiceSettings.ReaderQuotasMaxNameTableCharCount = 2147483647

$contentService.WcfServiceSettings["calendarservice.svc"] = $wcfServiceSettings
$contentService.WcfServiceSettings["commonservice.svc"] = $wcfServiceSettings
$contentService.WcfServiceSettings["departmentservice.svc"] = $wcfServiceSettings
$contentService.WcfServiceSettings["employeeservice.svc"] = $wcfServiceSettings
$contentService.WcfServiceSettings["shifttimeservice.svc"] = $wcfServiceSettings
$contentService.WcfServiceSettings["shiftmanagementservice.svc"] = $wcfServiceSettings
$contentService.WcfServiceSettings["changeshiftmanagementservice.svc"] = $wcfServiceSettings
$contentService.WcfServiceSettings["overtimeservice.svc"] = $wcfServiceSettings
$contentService.WcfServiceSettings["notoverTimemanagementservice.svc"] = $wcfServiceSettings
$contentService.WcfServiceSettings["vehiclemanagementservice.svc"] = $wcfServiceSettings
$contentService.WcfServiceSettings["leavemanagementservice.svc"] = $wcfServiceSettings
$contentService.WcfServiceSettings["freightmanagementservice.svc"] = $wcfServiceSettings
$contentService.WcfServiceSettings["businesstripmanagementservice.svc"] = $wcfServiceSettings

$contentService.Update($true)

Write-Host "End Update Custom Services... " -ForegroundColor Green

iisreset