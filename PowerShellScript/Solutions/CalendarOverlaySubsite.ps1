###############################################################################
# Creating Calendar overlays in SharePoint 2010 using Powershell
# Version          : 1.0
# Url              : http://khurrampunjwani.wordpress.com

###############################################################################

param ($SiteURL)

[string]$listName ="Calendar"
[string]$categories = "Holiday,Meeting,Weekend,Compensation Day-Off,On Leave,Other Events"
[array]$categoriesArray = $categories -split ","
$subSiteUrl =""
Function Get-SPCalendarWeb(){

     Add-PSSnapin Microsoft.Sharepoint.Powershell
     set-executionpolicy remotesigned

	#Clear screen first
	Clear-Host
	
	#Load web where Calendar is supposed to be created
 	$web = Get-SPWeb -Identity $SiteURL
    $Departments = $web.lists["Departments"]
    $spDepartmentItems = $Departments.Items 

    $spDepartmentItems | ForEach-Object {
	 	Write-Host "Subsite " $web.Title
        $subSiteUrl = $SiteURL + "/" + $_['Code']
		Write-Host "Subsite " $subSiteUrl
        $subSite =  Get-SPWeb -Identity $subSiteUrl
        New-SPCalendar
    }
	$web.Dispose()
}

Function New-SPCalendar(){
      [array]$ListArray = $listName -split ","

      foreach($list in $ListArray)
      {

	        $cal = $subSite.Lists.TryGetList($list);
	
	        Write-Host "Calendar created"
	
	        #Delete default categories and add the ones specified in command line
	        Add-CalendarCategories $cal
	
	        #Add Views with Filters
	        Add-SPCalendarViews $cal

     }
}

Function Add-SPCalendarViews($cal){
	$view = $cal.Views["Custom"]
	$viewFields = $view.ViewFields.ToStringCollection()
	$viewData = "<FieldRef Name='Title' Type='CalendarMonthTitle'/>"
    $viewData += "<FieldRef Name='Title' Type='CalendarWeekTitle'/>"
    $viewData += "<FieldRef Name='Location' Type='CalendarWeekLocation'/>"
    $viewData += "<FieldRef Name='Title' Type='CalendarDayTitle'/>"
    $viewData += "<FieldRef Name='Location' Type='CalendarDayLocation'/>"
	#XML for calendar overlays
	$calendarSettings = " <AggregationCalendars> "
	#To show different color for category views
	$colorIndex = 1 
	$isExistedView = $true
	#Create views for categories so there will be one view for each category
	
	foreach($category in $categoriesArray){

        $tempView = $cal.Views[$category]

        if( $tempView -eq $null )
        {
			Write-Verbose "Create View Calendar"
			#$isExistedView = $false
			#Create filter for the view to show events from the respective category
			$viewQuery = "<Where>"
			$viewQuery += "<And>"	
			$viewQuery += "<DateRangesOverlap>"
			$viewQuery += "<FieldRef Name='EventDate' />"
			$viewQuery += "<FieldRef Name='EndDate' />"
			$viewQuery += "<FieldRef Name='RecurrenceID' />"
			$viewQuery += "<Value Type='DateTime'><Month /></Value>"
			$viewQuery += "</DateRangesOverlap>"
			$viewQuery += "<Eq>"
			$viewQuery += "<FieldRef Name='Category'/>"
			$viewQuery += %{"<Value Type='Text'>{0}</Value>" -f $category}
			$viewQuery += "</Eq>"
			$viewQuery += "</And>"
			$viewQuery += "</Where>"
	
			$newView = $cal.Views.Add($category,$viewFields,$viewQuery,3,$false,
				$false,"CALENDAR",$false)
			$newView.ViewData = $viewData
			$newView.Update()

			%{"View created for {0} category" -f $category} | Write-Host
			$url = [uri]::EscapeDataString($category)
			$url = $url  -replace "-",""
			Write-Host $url
			#Update XML for Calendar Overlay, each view would be added as an overlay
			$calendarSettings += %{" <AggregationCalendar Id='{0}'" -f [Guid]::NewGuid()}
			$calendarSettings += " Type='SharePoint' "
			$calendarSettings += %{" Name='{0}'" -f $category }
			$calendarSettings += " Description=''"
			$calendarSettings += %{" Color='{0}' " -f $colorIndex}
			$calendarSettings += " AlwaysShow='True' "
			$calendarSettings += %{" CalendarUrl='{0}.aspx'> " -f $url}
			$calendarSettings += %{" <Settings WebUrl='{0}' " -f $subSiteUrl}
			$calendarSettings += %{" ListId='{0}'" -f $cal.ID}
			$calendarSettings += %{" ViewId='{0}'" -f $newView.ID}
			$calendarSettings += " ListFormUrl='DispForm.aspx' /> "
			$calendarSettings += " </AggregationCalendar> "
	
			#Change color for the next category
			$colorIndex += 1 
        }
		else
		{
			%{"View created for {0} category" -f $category} | Write-Host
			$url = [uri]::EscapeDataString($category)
			$url = $url  -replace "-",""
			Write-Host $url
			#Update XML for Calendar Overlay, each view would be added as an overlay
			$calendarSettings += %{" <AggregationCalendar Id='{0}'" -f [Guid]::NewGuid()}
			$calendarSettings += " Type='SharePoint' "
			$calendarSettings += %{" Name='{0}'" -f $category }
			$calendarSettings += " Description=''"
			$calendarSettings += %{" Color='{0}' " -f $colorIndex}
			$calendarSettings += " AlwaysShow='True' "
			$calendarSettings += %{" CalendarUrl='{0}.aspx'> " -f $url}
			$calendarSettings += %{" <Settings WebUrl='{0}' " -f $subSiteUrl}
			$calendarSettings += %{" ListId='{0}'" -f $cal.ID}
			$calendarSettings += %{" ViewId='{0}'" -f $tempView.ID}
			$calendarSettings += " ListFormUrl='DispForm.aspx' /> "
			$calendarSettings += " </AggregationCalendar> "
	
			#Change color for the next category
			$colorIndex += 1 
		}
	}
	
	#if(!$isExistedView){
		$calendarSettings += " </AggregationCalendars> "		
		Write-Verbose "Calendar Settings $calendarSettings"

		#Change the default view to show events which have no category associated
		#Otherwise it would show duplicates because of overlays
		$viewQuery = "<Where>"
		$viewQuery += "<IsNull>"
		$viewQuery += "<FieldRef Name='Category'/>"
		$viewQuery += "</IsNull>"
		$viewQuery += "</Where>"

		$view.CalendarSettings = $calendarSettings
		$view.Query = $viewQuery
		$view.Update()
	
		Write-Host "Calendar overlays created"	
		$cal.Update()
   #}
}

Function Add-CalendarCategories($cal){

	#Remove default out of the box categories
	$categoryField = $cal.Fields["Category"]
	$categoryField.Choices.Clear()
	
	#Add categories specified in command line
	$categoryField.Choices.AddRange($categoriesArray);
	$categoryField.Update();
	
	Write-Host "Category choices updated with" $categoryField.Choices
}

Get-SPCalendarWeb