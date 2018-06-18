Param($SiteURL, $DirPath);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

if ($SiteURL -eq $null) 
{ 
	$SiteURL = "http://localhost"
}

if ($DirPath -eq $null) 
{ 
	$DirPath = ".\CSVFile"
}

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}
set-executionpolicy remotesigned

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main($SiteURL)
{
	$filePath = $DirPath + "\CreateGroup.csv"
    Write-Host " "
	Write-Host "START CREATE GROUP" -ForegroundColor BLUE

    if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		$web = Get-SPWeb $SiteURL
		$groupCsvFilePath = get-item $filePath
		Write-Host "Path of file :: " $groupCsvFilePath -ForegroundColor DarkCyan
		Write-Host "Reading " $groupCsvFilePath " file" -ForegroundColor Green

		foreach($item in Import-Csv $groupCsvFilePath)
		{
            CreateGroupFromCSV $web $item
		}
	}
    $web.Dispose();
}

function CreateGroupFromCSV($web, $item)
{
    if($item.Group -ne "" -AND $item.PermissionLevel -ne "") 
    {
        Write-Host " "  
        Write-Host "==========================================="
        CreateGroup $web $item.Group $item.PermissionLevel
    }
    else
    {
        Write-Host "Input data invalid" 
    }
}

function CreateGroup($web, $SPGroupName, $PermissionLevel)
{
	try{
		Write-Host ">> Create group " $SPGroupName

		$group = $web.SiteGroups[$SPGroupName];
		#1. Check Group Existed
		if ($group -eq $null)
		{
			#2. If not to Create Group
			$NewSPGroup = $web.SiteGroups.Add($SPGroupName, $web.CurrentUser, $web.CurrentUser, $SPGroupName)
			Write-Host "Group is created successfully!!!" -ForegroundColor Green 
		}
		else
		{
			#Ignore
			Write-Host -ForegroundColor Yellow "Group is existed"
		}
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Create group $SPGroupName : $_" |Out-File $currentDirectory"\PowerShellLogs\CreateGroup.txt" -Append;   
		Write-Host  "Error - Create group $SPGroupName : $_" -ForegroundColor Red
	}

    AssignPermissionLevel $web $SPGroupName $PermissionLevel
}

 function AssignPermissionLevel($web, $SPGroupName, $PermissionLevel)
 {
	 try{
		 Write-Host ">> Assign basic permission for group " $SPGroupName
		 $group = $web.SiteGroups[$SPGroupName]
		 $role = $web.RoleDefinitions[$PermissionLevel]

		 $RoleAssignment = New-Object Microsoft.SharePoint.SPRoleAssignment($group)
		 $RoleAssignment.RoleDefinitionBindings.Add($role)

		 $web.RoleAssignments.Add($RoleAssignment)
		 $web.Update()
		 Write-Host "Assign permission successfully!!!"  -ForegroundColor Green 
		 }
	 catch
	 {
		 $currentDate = Get-Date
		"$currentDate : Error - Assign permission for group $group : $_" |Out-File $currentDirectory"\PowerShellLogs\CreateGroup.txt" -Append;   
		Write-Host  "Error - Assign permission for group $group : $_" -ForegroundColor Red
	 }
}

Start-Transcript

Main $SiteURL $DirPath

Write-Host "-- Done --"
Stop-Transcript

Remove-PsSnapin Microsoft.SharePoint.PowerShell