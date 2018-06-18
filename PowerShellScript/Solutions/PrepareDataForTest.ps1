Param($SiteURL);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
   Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}
set-executionpolicy remotesigned

Import-Module ActiveDirectory

#REMOVE employee

Add-PSSnapin "Microsoft.SharePoint.PowerShell"


function Main($SiteURL)
{
	if ($SiteURL -eq $null )
	{
	   $SiteURL="http://localhost"
	}
	
	$web = get-spweb $SiteURL 
		
	#UpdateADUserEmail
    UpdateEmployeeEmail $web
    #DeleteOvertime $web
    #DeleteOvertimeDetail $web
	

    $web.Dispose()
}

function DeleteEmployee($web)
{
	$list = $web.Lists["Employees"]
	#$list = $web.lists | where {$_.title -eq "Employees"}
	$items = $list.items
	foreach ($item in $items)
	{
		Write-host "  Say Goodbye to $($item.id)" -foregroundcolor red
		$list.getitembyid($Item.id).Delete()
	} 
}

function DeleteOvertime($web)
{
	$list = $web.Lists["Overtime Management"]
	#$list = $web.lists | where {$_.title -eq "Employees"}
	$items = $list.items
	foreach ($item in $items)
	{
		Write-host "  Say Goodbye to $($item.id)" -foregroundcolor red
		$list.getitembyid($Item.id).Delete()
	} 
}

function DeleteOvertimeDetail($web)
{
	$list = $web.Lists["Overtime Employee Details"]
	#$list = $web.lists | where {$_.title -eq "Employees"}
	$items = $list.items
	foreach ($item in $items)
	{
		Write-host "  Say Goodbye to $($item.id)" -foregroundcolor red
		$list.getitembyid($Item.id).Delete()
	} 
}

function UpdateEmployeeEmail($web)
{
	$employeeList = $web.Lists["Employees"]
	foreach ($emp in $employeeList.Items)
	{
		$emp[“Email”] = "tri.ngominh@vn.bosch.com"
		Write-Host $emp["Email"]
		$emp.update()
	}
	$employeeList.update()
}

function UpdateADUserEmail()
{
	$users = Get-ADUser -Filter *
	foreach ($user in $users)
	{
		$email = $user.samaccountname + '@sprbvh.com'
		Write-Host $email
		Set-ADUser -Identity $user -EmailAddress "tri.tngominh@vn.bosch.com"
		write-host $user.Name
	}  
}


Start-Transcript  
Write-Host "START PREPARE DATA FOR TEST" -ForegroundColor Blue
Main $SiteURL 
Write-Host "PREPARE DONE" -ForegroundColor Blue
Stop-Transcript

Remove-PsSnapin Microsoft.SharePoint.PowerShell