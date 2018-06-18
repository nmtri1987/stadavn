Param($SiteURL);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

if ($SiteURL -eq $null )
{
	$SiteURL="http://localhost"
}
$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function SetReadPermissionLevelForAllLists()
{
	$SPWeb = Get-SPWeb $SiteURL
    $lists=$SPWeb.Lists #| select Title,BaseTemplate, TypeId 
    $SPRoleREAD = "Read"

    foreach ($list in $lists)
    {
        Add-AllGroupSPPermissionToList $SPWeb $list $SPRoleREAD
    }
	$SPWeb.Dispose()
}

function Add-AllGroupSPPermissionToList ($web, $list, $permissionLevel)
{
    #$list = $web.Lists.TryGetList($ListName)
    if ($list -ne $null)
    {
        # Ensure that the permissions are not being inherited.
        if ($list.HasUniqueRoleAssignments -eq $False)
        {
            $list.BreakRoleInheritance($True)
        }

        # Modify the permissions.
		try {
			ForEach ($group in $web.SiteGroups) {                    
				$roleAssignment = new-object Microsoft.SharePoint.SPRoleAssignment($group)
				$roleDefinition = $web.RoleDefinitions[$permissionLevel];
				$roleAssignment.RoleDefinitionBindings.Add($roleDefinition);
				$list.RoleAssignments.Add($roleAssignment)
				#Write-Host "Adding $PermissionLevel permission to group $group.Name in $list list. " -foregroundcolor Yellow
			}
			$list.Update();
			Write-Host "Added $PermissionLevel permission for all group in $list list. " -foregroundcolor Green
		}
		catch
        {
			$currentDate = Get-Date
		    "$currentDate : Error - Error add $PermissionLevel permission for $group in $list. Exception: $_.Exception.Message" |Out-File $currentDirectory"\PowerShellLogs\AssignReadPermissionForAllLists.txt" -Append;   
		    Write-Host "Error add $PermissionLevel permission for $group in $list : $_" -ForegroundColor Red
	    }
    }
    else
    {
        Write-Host "List $list does not exist." -foregroundcolor Yellow
    }
}


Start-Transcript
Write-Host "START ASSIGN READ PERMISSION FOR ALL LISTS" -ForegroundColor Blue
SetReadPermissionLevelForAllLists
Write-Host "DONE" -ForegroundColor Blue
Stop-Transcript

