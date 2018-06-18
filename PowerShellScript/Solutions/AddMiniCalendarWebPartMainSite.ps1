 

param ($SiteURL)

 Add-PSSnapin Microsoft.Sharepoint.Powershell
 $web = Get-SPWeb $SiteURL 
 $lists ="Company Calendar - Location 2,Company Calendar - Location 1"
 $pageNameUrl = "/SitePages/Home.aspx"

 $page = $web.GetFile($pageNameUrl) 
 $page.CheckOut()  
 $webpartmanager = $web.GetLimitedWebPartManager($pageNameUrl, [System.Web.UI.WebControls.WebParts.PersonalizationScope]::Shared)    
 [array]$listNameArray = $lists -split ","
 $currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition
 Function CreateCalendarWebpart()
 {
 try
	{
    RemoveWebpart

    #Initial WebPart

     foreach($listName in $listNameArray)
     {

            $list = $web.Lists[$listName];
            $ListViewWebPart = New-Object Microsoft.SharePoint.WebPartPages.XsltListViewWebPart
            $ListViewWebPart.Title = $listName
            $ListViewWebPart.ListName = ($list.ID).ToString("B").ToUpper()
            Write-Host ($list.ID).ToString("B").ToUpper()
            $ListViewWebPart.ViewGuid = ($list.DefaultView.ID).ToString("B").ToUpper()
            Write-Host ($list.DefaultView.ID).ToString("B").ToUpper()
            $ListViewWebPart.ZoneID = "Row1_right"
            $ListViewWebPart.view
            $ListViewWebPart.AllowEdit=$false;
            $ListViewWebPart.TitleUrl = $list.DefaultViewUrl
            $ListViewWebPart.WebId = $list.ParentWeb.ID
            $webpartmanager.AddWebPart($ListViewWebPart, "Right", "0")
            $webpartmanager.DeleteWebPart($ListViewWebPart)

            $list = $web.Lists[$listName];
            $ListViewWebPart = New-Object Microsoft.SharePoint.WebPartPages.ListViewWebPart
            $ListViewWebPart.Title = $listName
            $ListViewWebPart.ListName = ($list.ID).ToString("B").ToUpper()
            Write-Host ($list.ID).ToString("B").ToUpper()
            $ListViewWebPart.ViewGuid = ($list.DefaultView.ID).ToString("B").ToUpper()
            Write-Host ($list.DefaultView.ID).ToString("B").ToUpper()
            $ListViewWebPart.ZoneID = "Row1_right"
            $ListViewWebPart.view
            $ListViewWebPart.AllowEdit=$false;
            $ListViewWebPart.TitleUrl = $list.DefaultViewUrl
            $ListViewWebPart.WebId = $list.ParentWeb.ID
            $ListViewWebPart.PartOrder=1
            $webpartmanager.AddWebPart($ListViewWebPart, "Row_right", "0")
     }

    $page.CheckIn("Updated")  
    $web.Update();   
    $web.Dispose();  
    }
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - CreateCalendarWebpart : $_" |Out-File $currentDirectory"\PowerShellLogs\AddMiniCalendarWebPartMainSite.txt" -Append;   
		Write-Host  "Error - Error - CreateCalendarWebpart : $_" -ForegroundColor Red
	}
}
Function RemoveWebpart()
{ 

 try
	{

             $web = Get-SPWeb  $SiteURL

             $page = $web.GetFile($pageNameUrl) 

             $webpartmanager = $web.GetLimitedWebPartManager($pageNameUrl, [System.Web.UI.WebControls.WebParts.PersonalizationScope]::Shared)

             $collectionVariable = New-Object System.Collections.ArrayList


              foreach ($webpart in $webpartmanager.WebParts)
              {
                 foreach($title in $listNameArray){

                    if($title -eq $webpart.Title){

                         $collectionVariable.Add($webpart)
                    }
                 }

              }
              foreach($item in  $collectionVariable){

                      write-host $item.Title
                      $webpartmanager.DeleteWebPart($item)   
              }

              $web.Update();   
              $web.Dispose();  
     
     }
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Add mini calendar main site : $_" |Out-File $currentDirectory"\PowerShellLogs\AddMiniCalendarWebPartMainSite.txt" -Append;   
		Write-Host  "Error - Error - Add mini calendar main site : $_" -ForegroundColor Red
	}
}
RemoveWebpart
CreateCalendarWebpart