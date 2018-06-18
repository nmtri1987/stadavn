Param($SiteURL);

# This script apply for BOD and System Admin group only

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

if ($SiteURL -eq $null )
{
	$SiteURL="http://windev162:1111/"
}

function SetContributePermissionLevelForLists()
{
	$SPWeb = Get-SPWeb $SiteURL
	$filePath = $currentDirectory + "\CSVFile\ContributorPermissionByModule.csv"
	$moduleInfors = Import-Csv $filePath
    $PermissionLevel = "Contribute"
    $departmentList = $SPWeb.Lists["Departments"]

	foreach($module in $moduleInfors)
    {
        $list = $SPWeb.Lists[$module.ListName]
        if($list -ne $null) {

            $groupToAdds = $module.DepartmentGroup.Split(";");
            if($module.Department -eq $null -OR $module.Department -eq "")
             {
                foreach($department in $departmentList.Items)
                {
                    foreach($groupToAdd in $groupToAdds)
                    {
                        $groupName = $department["Name"] +" "+ $groupToAdd
                        Add-SPPermissionToListGroup $SPWeb $list $groupName $module.PermissionLevel
                    }
                }
            }
            else
            {
                foreach($groupToAdd in $groupToAdds)
                {
                    $groupName = $module.Department +" "+ $groupToAdd
                    Add-SPPermissionToListGroup $SPWeb $list $groupName $module.PermissionLevel
                }
            }
            $list.Update()
        }
	}
	$SPWeb.Dispose()
}

function Add-SPPermissionToListGroup ($web, $list, $groupName, $permissionLevel)
{
    # Ensure that the permissions are not being inherited.
    if ($list.HasUniqueRoleAssignments -eq $False)
    {
        $list.BreakRoleInheritance($True)
    }

    # Modify the permissions.
	try {
		$group = $web.SiteGroups[$groupName]
		$roleAssignment = new-object Microsoft.SharePoint.SPRoleAssignment($group)
		$roleDefinition = $web.RoleDefinitions[$permissionLevel];
		$roleAssignment.RoleDefinitionBindings.Add($roleDefinition);
		$list.RoleAssignments.Add($roleAssignment)
		Write-Host "Added $group $permissionLevel permission for all group in $list list. " -foregroundcolor Green
	}
	catch
    {
		$currentDate = Get-Date
		"$currentDate : Error - Error add $groupName to list $list. Exception: $_" |Out-File $currentDirectory"\PowerShellLogs\AssignContributorPermissionByModule.txt" -Append;   
		Write-host "Error $groupName : $_" -ForegroundColor Red
	}
}

Start-Transcript
Write-Host "START ASSIGN PERMISSION BY MODULE" -ForegroundColor Blue
SetContributePermissionLevelForLists
Write-Host "DONE" -ForegroundColor Blue
Stop-Transcript

