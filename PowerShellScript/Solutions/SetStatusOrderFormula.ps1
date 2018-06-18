Param($SiteURL,$ListName);
Add-PSSnapin Microsoft.Sharepoint.Powershell
[System.Reflection.Assembly]::LoadWithPartialName(”Microsoft.SharePoint”) 
$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main($SiteURL, $ListName){
		
		if( $SiteURL -eq $null)
		{
			$SiteURL = "http://windev162:1111"
		}
        
		if( $ListName -eq $null)
		{
			#$ListName = "Employee Requirement Sheets"
			#$ListName = "Request For Diploma Supplies"
			$ListName = "Requests"
		}
		Write-Host $SiteURL
		$spWeb = get-spweb $SiteURL 
		
		
		$spList = $spWeb.Lists[$listName]
		if($spList -ne $null)
		{
				$collFields = $spList.Fields["Status Order"];
                #$formula = '=IF([Workflow status]="In-Progress","0",IF([Workflow status]="Approved","1",IF([Workflow status]="Rejected","2",IF([Workflow status]="Cancelled","3","4"))))'
				$formula = '=IF([Workflow status]="In-Progress","0",IF([Workflow status]="In-Process","1",IF([Workflow status]="Completed","2",IF([Workflow status]="Rejected","3",IF([Workflow status]="Cancelled","4","5"))))'
				$collFields.Formula = $formula
				$collFields.Update();
				$spList.Update();
		}


		
		$spWeb.Dispose()
}

Start-Transcript  
Write-Host "START SET FORMULA STATUS ORDER COLUMN" -ForegroundColor Blue
Main $SiteURL $ListName
Write-Host "DONE" -ForegroundColor Blue
Stop-Transcript

Remove-PsSnapin Microsoft.SharePoint.PowerShell

