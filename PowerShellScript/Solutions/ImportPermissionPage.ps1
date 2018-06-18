Param($SiteURL,$FilePath);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main($SiteURL,$FilePath){

		if($SiteURL -eq $null){
			$SiteURL="http://localhost"
		}
		if($FilePath -eq $null)
		{
			$FilePath = $currentDirectory + "\CSVFile\GroupPermission.csv"
		}
		
		Write-Host $SiteURL
		$web = get-spweb $SiteURL 
		$groupList = $web.Lists["Groups"]
		$PermissionGroupList = $web.Lists["Permission Group"]
	    $CategoryModule = $web.Lists["Module Category"]
		$source = Import-Csv -Delimiter "," -Path $FilePath
    
        foreach ($item in $source)
		{
			try{
				$qryPermission = new-object Microsoft.SharePoint.SPQuery
				$qryPermission.Query = "<Where><Eq> <FieldRef Name='PageName' /><Value Type='Text'>"+$item.Pagename.Trim()+"</Value></Eq></Where>"
				$dataResults = $PermissionGroupList.getItems($qryPermission)
				$stringItems = Get-GroupAccessString $groupList  $item.Groups

				$query1 = New-Object  Microsoft.SharePoint.SPQuery 
				$query1.query =" <Where><Eq><FieldRef Name='CommonName' /><Value Type='Text'>"+$item.Category+"</Value></Eq></Where>"
				$items1 = $CategoryModule.getItems($query1)

				if($dataResults.Count -eq 0)
				{
				    Write-Host "Menu " $item.IsOnLeftMenu
                 
					$newItem = $PermissionGroupList.AddItem()
					$newitem["PageName"] = $item.Pagename
					Write-Host $itemValues.Count
					$stringItems = Get-GroupAccessString $groupList  $item.Groups
					$newitem["GroupAccess"]= $stringItems
					$newitem["IsOnLeftMenu"]= $item.IsOnLeftMenu
					$newitem["CommonName"]= $item.CommonName
					$newitem["LeftMenuOrder"]= $item.Order
					$newitem["CommonName1066"]= $item.CommonName1066
					$newitem["PermissionModuleCategory"] =$items1[0].ID
					$newitem.Update()
				}
				else
				{
					Write-Host "Update"
					$itemData = $dataResults[0]
					$itemData["GroupAccess"]= $stringItems
					$itemData["IsOnLeftMenu"]= $item.IsOnLeftMenu
					$itemData["CommonName"]= $item.CommonName
					$itemData["LeftMenuOrder"]= $item.Order
					$itemData["CommonName1066"]= $item.CommonName1066
					$itemData["PermissionModuleCategory"] =$items1[0].ID
					$itemData.Update()
				}
			}
			catch
			{
				$currentDate = Get-Date
				"$currentDate : Error - Import permission group $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportPermissionPage.txt" -Append;   
				Write-Host "Error : " $_ -ForegroundColor Red
			}
		}
        $PermissionGroupList.Update()
	    #$rootWeb.Dispose()
		$web.Dispose()
		#$site.Dispose()
}

function Get-GroupAccessString($groupList, $groupString)
{
    $groups = $groupString.Split("{;}")
	$itemValues =new-object Microsoft.SharePoint.SPFieldLookupValueCollection

	$groups | ForEach {
			$qry = new-object Microsoft.SharePoint.SPQuery
			$qry.Query = "<Where><Eq> <FieldRef Name='CommonName' /><Value Type='Text'>"+$_+"</Value></Eq></Where>"
			$groupData = $groupList.getItems($qry)
			#Write-Host $_ -ForegroundColor Green
			if($groupData.Count -gt 0)
			{
             
                if($groupData[0]["CommonName"] -like '*Contributors*')
                {
				    Write-Host $groupData[0].ID - $groupData[0]["CommonName"] -ForegroundColor DarkMagenta
                }
                if($groupData[0]["CommonName"] -like '*Members*')
                {
				    Write-Host $groupData[0].ID - $groupData[0]["CommonName"] -ForegroundColor Darkgreen
                }
                 if($groupData[0]["CommonName"] -like '*Administrators*')
                {
				    Write-Host $groupData[0].ID - $groupData[0]["CommonName"] -ForegroundColor DarkCyan
                }
				$lookupValue = New-Object Microsoft.SharePoint.SPFieldLookupValue($groupData[0].ID,$groupData[0].ID);
				$itemValues.Add($lookupValue)
			}
	}

	return $itemValues.ToString()
}


Start-Transcript  
Write-Host "START IMRPORT PERMISSION GROUP DATA" -ForegroundColor Blue
Main $SiteURL $FilePath
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell