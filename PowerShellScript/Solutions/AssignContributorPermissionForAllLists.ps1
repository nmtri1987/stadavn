Param($SiteURL);

# This script apply for BOD and System Admin group only

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

if ($SiteURL -eq $null )
{
	$SiteURL="http://localhost"
}

function SetContributePermissionLevelForAllLists()
{
	$SPWeb = Get-SPWeb $SiteURL
	$filePath = $currentDirectory + "\CSVFile\GroupHasContributorInAllLists.csv"
	$groupsInfo = Import-Csv $filePath
    $lists = $SPWeb.Lists #| select Title,BaseTemplate, TypeId 
    $PermissionLevel = "Contribute"

    foreach ($list in $lists)
    {
		foreach($groupToAdd in $groupsInfo)
        {
			Add-SPPermissionToListGroup $SPWeb $list $groupToAdd.Group $PermissionLevel
		}
		$list.Update();
    }
	$SPWeb.Dispose()
}

function Add-SPPermissionToListGroup ($web, $list, $groupName, $permissionLevel)
{
    if ($list -ne $null)
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
		    "$currentDate Error - Error add $groupName to list $list. Exception: $_" |Out-File $currentDirectory"\PowerShellLogs\AssignContributorPermissionForAllLists.txt" -Append;   
		    Write-Host "Error : $groupName - $_" -ForegroundColor Red
	    }
    }
    else
    {
        Write-Host "List $list does not exist." -foregroundcolor Yellow
    }
}

Start-Transcript
Write-Host "START ASSIGN CONTRIBUTE PERMISSION FOR ALL LISTS" -ForegroundColor Blue
SetContributePermissionLevelForAllLists
Write-Host "DONE" -ForegroundColor Blue
Stop-Transcript

