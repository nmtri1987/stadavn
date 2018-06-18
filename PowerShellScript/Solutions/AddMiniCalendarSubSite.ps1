param ($SiteURL)

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

if($SiteURL -eq $null)
{
    $SiteURL = "http://tronghieusp"
}

$mainWeb = Get-SPWeb $SiteURL 
$lists ="Calendar"
$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

 Function CreateCalendarWebpart()
 {
 try
	{


    $Departments = $mainWeb.lists["Departments"]
    $spDepartmentItems = $Departments.Items 

    
    $spDepartmentItems | ForEach-Object {

             $subUrl = $SiteURL + "/" + $_['Code']

             $web = Get-SPWeb  $subUrl

             $pageNameUrl = "SitePages/Home.aspx"

             $page = $web.GetFile($pageNameUrl) 

             $page.CheckOut()  

             $webpartmanager = $web.GetLimitedWebPartManager($pageNameUrl, [System.Web.UI.WebControls.WebParts.PersonalizationScope]::Shared)    


                    $list = $web.Lists[$lists];
                    $ListViewWebPart = New-Object Microsoft.SharePoint.WebPartPages.XsltListViewWebPart
                    $ListViewWebPart.Title = $listName
                    $ListViewWebPart.ListName = ($list.ID).ToString("B").ToUpper()
                    Write-Host ($list.ID).ToString("B").ToUpper()
                    $ListViewWebPart.ViewGuid = ($list.DefaultView.ID).ToString("B").ToUpper()
                    Write-Host ($list.DefaultView.ID).ToString("B").ToUpper()
                    $ListViewWebPart.ZoneID = "Row_right"
                    $ListViewWebPart.view
                    $ListViewWebPart.AllowEdit=$false;
                    $ListViewWebPart.TitleUrl = $list.DefaultViewUrl
                    $ListViewWebPart.WebId = $list.ParentWeb.ID
                    $webpartmanager.AddWebPart($ListViewWebPart, "Row_right", "0")
                    $webpartmanager.DeleteWebPart($ListViewWebPart)

                    $list = $web.Lists[$lists];
                    $ListViewWebPart = New-Object Microsoft.SharePoint.WebPartPages.ListViewWebPart
                    $ListViewWebPart.Title = $listName
                    $ListViewWebPart.ListName = ($list.ID).ToString("B").ToUpper()
                    Write-Host ($list.ID).ToString("B").ToUpper()
                    $ListViewWebPart.ViewGuid = ($list.DefaultView.ID).ToString("B").ToUpper()
                    Write-Host ($list.DefaultView.ID).ToString("B").ToUpper()
                    $ListViewWebPart.ZoneID = "Row_right"
                    $ListViewWebPart.view
                    $ListViewWebPart.AllowEdit=$false;
                    $ListViewWebPart.TitleUrl = $list.DefaultViewUrl
                    $ListViewWebPart.WebId = $list.ParentWeb.ID
                    $ListViewWebPart.PartOrder=1
                    $webpartmanager.AddWebPart($ListViewWebPart, "Row_right", "0")
          

            $page.CheckIn("Updated")  
            $web.Update();   
            $web.Dispose();  
    }
    }
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Add mini calendar sub site : $_" |Out-File $currentDirectory"\PowerShellLogs\AddMiniCalendarSubSite.txt" -Append;   
		Write-Host  "Error - Error - Add mini calendar sub site : $_" -ForegroundColor Red
	}

}
Function RemoveWebpart()
{ 
try 
	{

    $Departments = $mainWeb.lists["Departments"]
    $spDepartmentItems = $Departments.Items 

    
    $spDepartmentItems | ForEach-Object {

             $subUrl = $SiteURL  + "/" + $_['Code']

             $web = Get-SPWeb  $subUrl

             $pageNameUrl = "SitePages/Home.aspx"

             $page = $web.GetFile($pageNameUrl) 

             $page.CheckOut()  

             $webpartmanager = $web.GetLimitedWebPartManager($pageNameUrl, [System.Web.UI.WebControls.WebParts.PersonalizationScope]::Shared)

              $webparttem  = New-Object Microsoft.SharePoint.WebPartPages.ListViewWebPart

              foreach ($webpart in ($webpartmanager.WebParts | Where-Object {$_.Title -eq $lists}))
              {
   
                $webparttem =  $webpart
                $webpartmanager.DeleteWebPart($webparttem)   
    
              }
    

              $page.CheckIn("Updated")  
              $web.Update();   
              $web.Dispose();  
     }  

     }
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - remove web part sub site : $_" |Out-File $currentDirectory"\PowerShellLogs\cl.txt" -Append;   
		Write-Host  "Error - remove web part sub site : $_" -ForegroundColor Red
	}
}
RemoveWebpart
CreateCalendarWebpart