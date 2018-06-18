if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}


function Delete-ChildWeb($ParentWeb){
    if($ParentWeb.Webs.Count -eq 0 -and -not $ParentWeb.IsRootWeb){
    Write-Host "Deleting Web '$ParentWeb'"
    $ParentWeb.Delete()
    }
    else{
        foreach($childWeb in $ParentWeb.Webs){
        Delete-ChildWeb $childWeb
    }
    if(-not $ParentWeb.IsRootWeb){
        Write-Host "Deleting Web '$ParentWeb'"
        $ParentWeb.Delete()
     }
   }
}

Start-Transcript
$DeleteSiteCollection = $false
$SiteUrl ="http://localhost:1111"
if($DeleteSiteCollection){
    Remove-SPSite –Identity $SiteUrl –GradualDelete –Confirm:$False
}
else{
    $Site = Get-SPSite $SiteUrl
    $Web = $Site.RootWeb
    Delete-ChildWeb $Web|
    Write-Host "Web '$Web' deleted successfully" -ForegroundColor Green
}
Stop-Transcript