Param($SiteURL);
Write-Host $SiteURL -ForegroundColor DarkGreen

Add-PsSnapin Microsoft.SharePoint.PowerShell
set-executionpolicy remotesigned

## SharePoint DLL 
[void][System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SharePoint") 

#Creating Sub Sites in top site collection.
Write-Output " "
Write-Output "Checking for template..."

$SolutionURLForUploadTemplate = $SiteURL #Forexample:  http://sp-devbox2013
$SiteCollectionURL = $SiteURL #Forexample:  http://sp-devbox2013

$site = new-Object Microsoft.SharePoint.SPSite($SiteCollectionURL)
$SiteCollectionLanguage = 1033

$loc= [System.Int32]::Parse(1033)
$templates= $site.GetWebTemplates($loc)

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

$SiteCollectionTemplate

foreach ($child in $templates)
{ 
    if($child.Title -eq "PoliciesSite")
    {
       $SiteCollectionTemplate = $child     
    }
}

if(!$SiteCollectionTemplate)
{
    Write-Output "Uploading New Template..."  

    $linkPath =  ($(get-location).Path).Trim() + "\SiteTemplates\PoliciesSite.wsp"
    
    Add-SPUserSolution -LiteralPath $linkPath -Site $SolutionURLForUploadTemplate 
    Install-SPUserSolution -Identity "PoliciesSite.wsp" -Site $SolutionURLForUploadTemplate 
    #Install-SPSolution
    Write-Output " "
    Write-Output "Checking For New Template..."
    $newSite = new-Object Microsoft.SharePoint.SPSite($SiteCollectionURL)
    $newTemplates = $newSite.GetWebTemplates($loc)

    foreach ($child1 in $newTemplates)
    {        
        if ($child1.Title -eq "PoliciesSite")
        {
            $SiteCollectionTemplate = $child1     
        }
    }
    if(!$SiteCollectionTemplate)
    {
        Write-Output -ForegroundColor Red "ERROR: Template not found!"
        Write-Output -ForegroundColor Red "STOPPED !"
        exit
    }
    else
    {
        Write-Host -ForegroundColor Green "New Template is valid"
    }
}
else
{
    Write-Host -ForegroundColor Green "Template is valid"
}

function ActiveFeature($featureId)
{
    try
    {
        Write-Output "Active feature " $featureId
        Enable-SPFeature -Identity $featureId -Url $SiteURL -ErrorAction Stop

        Write-Host -ForegroundColor Green "Feature is activated successfully! "
    }
    catch [System.Management.Automation.ActionPreferenceStopException]
    {
        if( !($_.Exception -is [System.Data.DuplicateNameException]) )
        {
            #Its not a "feature is already activated at scope" exception
        }
        else
        {
            #Handle the "feature is already activated at scope" exception
        }
        Write-Host -ForegroundColor Yellow "Feature is already activated at scope!"
    }
}

Write-Output "Active Features"
ActiveFeature "f6924d36-2fa8-4f0b-b16d-06b7250180fa" #PublishingSite
ActiveFeature "0af5989a-3aea-4519-8ab0-85d91abe39ff" #Workflows
ActiveFeature "a44d2aa3-affc-4d58-8db4-f4a3af053188" #Publishing Approval Workflows

ActiveFeature "17415b1d-5339-42f9-a10b-3fef756b84d1" #PublishingSite
ActiveFeature "7c637b23-06c4-472d-9a9a-7c175762c5c4" #

Write-Output "Creating SubSites with template: "  $SiteCollectionTemplate.Title

$SiteUrl = $SiteCollectionURL + "/Policies"
 
$siteName =  "Policies"
Write-Output " "
Write-Output "$order - Adding Site: $siteName ..."
    
$exists = (Get-SPWeb $SiteUrl  -ErrorAction SilentlyContinue) -ne $null
if($exists)
{
    Write-Host -ForegroundColor Yellow  "Site $siteName already existed. Skipped" 
    # Remove-SPWeb  $SiteUrl -Confirm:$false
}
else{
	try{
		Write-Output "Creating new site : $siteName ..." 
		$newWeb =  New-SPWeb -Url $SiteUrl -Name $siteName
		$newWeb.ApplyWebTemplate($SiteCollectionTemplate)
		$newWeb.Dispose();

		#Apply Master Page stada
		Write-Output "Apply master page"
		$newSite = new-Object Microsoft.SharePoint.SPSite($SiteUrl)
		$newWeb = $newSite.OpenWeb()
		$newWeb.MasterUrl = $site.RootWeb.ServerRelativeUrl +  "_catalogs/masterpage/stada.master"
		$newWeb.CustomMasterUrl = $site.RootWeb.ServerRelativeUrl + "_catalogs/masterpage/stada.master"
		$newWeb.Update();
		Write-Host -ForegroundColor Green "Created $siteName successfully !!! "
	}
	catch
    {
		$currentDate = Get-Date
		"$currentDate : Error - Create Policy site : $_" |Out-File $currentDirectory"\PowerShellLogs\CreatePoliciesSite.txt" -Append;   
		Write-host "Error - Create Policy site : $_" -ForegroundColor Red
	}
}
  
Write-Output "--DONE--"

Remove-PsSnapin Microsoft.SharePoint.PowerShell