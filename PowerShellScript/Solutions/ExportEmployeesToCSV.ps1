Param($ADServer , $SiteURL);

if ($ADServer -eq $null) {
	$ADServer='SPRBVHDEV';
}
if ($SiteURL -eq $null) {
	$SiteURL = "http://windev162:1111"
}

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

Add-PSSnapin Microsoft.Sharepoint.PowerShell
set-executionpolicy remotesigned

function Main(){
    ExportADFSUser 
}

function ExportADFSUser(){
	$FilePath = ".\CSVFile\ADFS_User.csv"
	$web = Get-SPWeb $SiteURL
    $departments = $web.Lists["Departments"]
	#$ADServer='SPRBVHDEV';
    #import the ActiveDirectory Module 
    Import-Module ActiveDirectory 
     
    #Perform AD search. The quotes "" used in $SearchLoc is essential 
    #Without it, Export-ADUsers returuned error 
                  Get-ADUser -server $ADServer -Properties * -Filter * |
                  Select-Object @{Label = "EmployeeType";Expression = {"AD User"}},  
				  @{Label = "FirstName";Expression = {$_.GivenName}},  
                  @{Label = "LastName";Expression = {$_.Surname}}, 
                  @{Label = "AD Account";Expression = {$_.SamAccountName}}, 
                  @{Label = "HomeTown";Expression = {$_.City}}, 
                  @{Label = "Address";Expression = {$_.StreetAddress}}, 
                  @{Label = "Position(Job Title)";Expression = {$_.Title}}, 
                  @{Label = "Manager";Expression = {$_.Manager.sAMAccountName}}, 
                  @{Label = "Department";Expression = {$_.Department}}, 
                  @{Label = "EmployeeID";Expression = {$_.EmployeeID}}, 
                  @{Label = "Email";Expression = {$_.Mail}}, 
                  @{Label = "Group";Expression = {$_.Group}} ,  
                  @{Label ="Missing Info";Expression ={
						Write-Host "Department: "  $_.Department
						$query1 = New-Object  Microsoft.SharePoint.SPQuery 
						$query1.query =" <Where><Eq><FieldRef Name='CommonName' /><Value Type='Text'>"+$_.Department+"</Value></Eq></Where>"
						$items1 = $departments.getItems($query1)
						if ($items1.Count -eq 0)
						{
							'Department not found'
						}
				   }}|
                  #Export CSV report 
                  Export-Csv -Path  $FilePath -NoTypeInformation    
write-host "---DONE---"
}

Start-Transcript  

write-host "START EXPORT DATA TO CSV" $SiteURL  -ForegroundColor Blue


Main

Stop-Transcript

pause