Param($SiteURL, $DirPath);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

 if ($SiteURL -eq $null )
{
	$SiteURL="http://localhost:81"
}

if ($DirPath -eq $null) 
{ 
	$DirPath = ".\CSVFile"
}

function AssignForRootSiteMain()
{
	$SPWeb = Get-SPWeb $SiteURL
    $SPRoleREAD = "Read"
	Write-Host Role Assign Site Root -ForegroundColor Green

 	$filePath = $DirPath + "\CreateGroup.csv"
	$groupCsvFilePath = get-item $filePath

	foreach($item in Import-Csv $groupCsvFilePath)
	{
		Write-Host $item.Group -ForegroundColor Green
		RoleAssignSiteRoot  $SPWeb  $item.Group  $SPRoleREAD
	}

	Write-Host " "
}

function RoleAssignSiteRoot($web, $SPGroupName, $SPRoleDefinitionName)
{
    #Check Role Assignments
    if ($web.RoleAssignments[$SPGroupName] -eq $null)
    {
        #Create Role Assignment
        $IsValidRoleAssignment = $true
        $SPGroup = $Web.SiteGroups[$SPGroupName]
        if ($SPGroup -eq $null)
        {
            Write-Host -ForegroundColor Yellow "Group is not found"
            $IsValidRoleAssignment = $false
        }
        else
        {
            $IsValidRoleAssignment = $IsValidRoleAssignment -and $true
        }

        $SPRoleDefinition = $Web.RoleDefinitions[$SPRoleDefinitionName]

        if ($SPRoleDefinition -eq $null)
        {
            Write-Host -ForegroundColor Yellow "Role definition is not found"
            $IsValidRoleAssignment = false
        }
        else
        {
            $IsValidRoleAssignment = $IsValidRoleAssignment -and $true
        }

		#Remove other permissions if any
		
        if ($IsValidRoleAssignment)
        {
            $SPRoleAssignment = New-Object Microsoft.SharePoint.SPRoleAssignment($SPGroup)
            $SPRoleAssignment.RoleDefinitionBindings.Add($SPRoleDefinition)

            $web.RoleAssignments.Add($SPRoleAssignment)
        }
        else
        {
            Write-Host -ForegroundColor Green "Assign role unsuccessfully"
        }
    }
    else
    {
        Write-Host -ForegroundColor Yello "Role is existed"
    }
}

Start-Transcript
AssignForRootSiteMain
Stop-Transcript