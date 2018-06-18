Param($SiteURL, $DirPath);

Write-Host $SiteURL
Write-Host $DirPath

Add-PsSnapin Microsoft.SharePoint.PowerShell
set-executionpolicy remotesigned

## SharePoint DLL 
[void][System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SharePoint") 

#Creating Sub Sites in top site collection.
Write-Output " "
Write-Output "Checking for template..."

$SiteCollectionURL = $SiteURL #Forexample:  http://sp-devbox2013/

$site = new-Object Microsoft.SharePoint.SPSite($SiteCollectionURL)


$SiteCollectionLanguage = 1033

$loc= [System.Int32]::Parse(1033)
$templates= $site.GetWebTemplates($loc)

$SiteCollectionTemplate

foreach ($child in $templates)
{ 
    if($child.Title -eq "PoliciesSite")
    {
       $SiteCollectionTemplate = $child     
    }
}

$PoliciesDirPath=$DirPath+"\PoliciesSite.wsp"
Write-Host $PoliciesDirPath
if(!$SiteCollectionTemplate)
{
    Write-Output "Uploading New Template..."  

    Add-SPUserSolution -LiteralPath $PoliciesDirPath -Site $SiteCollectionURL
    Install-SPUserSolution -Identity $PoliciesDirPath -Site $SiteCollectionURL
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

function SetLanguage()
{
	 foreach ($spWeb in $site.AllWebs)
	 {
	   $spWeb.IsMultilingual = $true
	   $WebRegionSettings = New-Object Microsoft.SharePoint.SPRegionalSettings($spWeb)
	   foreach ($language in $WebRegionSettings.InstalledLanguages)
	   {
		 If ($language.DisplayName -eq "English" -or $language.DisplayName -eq "Vietnamese")
		 # Add the displayname of any langauge you have installed: -or $language.DisplayName -eq "Norwegian" -or $language.DisplayName -eq "Finnish" -or $language.DisplayName -eq "Danish"
		 {
			write-host -BackgroundColor Green -ForegroundColor Black "Update -" $spWeb "site with LCID:" $language.DisplayName
			$culture = New-Object System.Globalization.CultureInfo($language.LCID)
			$spWeb.AddSupportedUICulture($Culture)
		 }
		 else
		 {
			Write-host " Language not activated: " $language.DisplayName " on site " $spWeb.Name
		 }
	   }
	   $spWeb.Update()
	 }
}

Write-Output "Active Features"
ActiveFeature "f6924d36-2fa8-4f0b-b16d-06b7250180fa" #PublishingSite
ActiveFeature "0af5989a-3aea-4519-8ab0-85d91abe39ff" #Workflows
ActiveFeature "a44d2aa3-affc-4d58-8db4-f4a3af053188" #Publishing Approval Workflows

Write-Output "Creating SubSites with template: "  $SiteCollectionTemplate.Title

$SiteUrl = $SiteCollectionURL + "Policies"
 
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

	# Enables SELECTED installed languages for each subsite in a site collection
    Write-Host -ForegroundColor Green "Created $siteName successfully !!! "
	
}

SetLanguage
  
Write-Output "--DONE--"

Remove-PsSnapin Microsoft.SharePoint.PowerShell