Param($SiteURL);
Add-PSSnapin Microsoft.Sharepoint.Powershell

function CheckIfGroupNameExist($list, $groupName)
{
	 $qryPermission = new-object Microsoft.SharePoint.SPQuery
			$qryPermission.Query = "<Where><Eq> <FieldRef Name='CommonName' /><Value Type='Text'>"+$groupName+"</Value></Eq></Where>"
			$dataResults = $list.getItems($qryPermission)
	if($dataResults.Count -eq 0)
	{
		return 0
	}
	else
	{
		return $dataResults.Count
	}    
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main($SiteURL){
		
		if( $SiteURL -eq $null)
		{
			$SiteURL = "http://localhost"
		}

		Write-Host $SiteURL
		$web = get-spweb $SiteURL 
		$site = $web.Site
		$rootWeb = $site.RootWeb
		$Output = @("GroupName")
		$groups = $web.SiteGroups
		$list = $web.Lists["Groups"]

		foreach ($groupName in $groups)
		{
			try{
					$isExisted = CheckIfGroupNameExist $list $groupName
					if($isExisted -eq 0)
					{
						#Add new 
						Write-Host "$groupName - Group Name not existed ==> Add new"
						$newItem = $list.items.add()
						$newitem["CommonName"] = $groupName
						$newitem.update()
					}
					else
					{
						Write-Host "$groupName - Group Name existed"
					}
				}
			catch
			{
				$currentDate = Get-Date
				"$currentDate : Error - Import group  $groupName : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportGroups.txt" -Append;   
				Write-Host "Error : " $_ -ForegroundColor Red
			}
		}
		$rootWeb.Dispose()
		$web.Dispose()
		$site.Dispose()
}

Start-Transcript  
Write-Host "START IMRPORT GROUPS DATA" -ForegroundColor Blue
Main $SiteURL
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript

Remove-PsSnapin Microsoft.SharePoint.PowerShell

