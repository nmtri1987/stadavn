Param($SiteURL,$FilePath);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

# enable event firing 
$myAss = [Reflection.Assembly]::LoadWithPartialName("Microsoft.SharePoint");
$type = $myAss.GetType("Microsoft.SharePoint.SPEventManager");
$prop = $type.GetProperty([string]"EventFiringDisabled",[System.Reflection.BindingFlags] ([System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Static));
$prop.SetValue($null, $false, $null);

if($SiteURL -eq $null){
	$SiteURL="http://localhost"
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main($SiteURL,$FilePath){

	if($FilePath -eq $null)
	{
		$FilePath = $currentDirectory + "\CSVFile\DepartmentList.csv"
	}
		
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		Write-Host $SiteURL -ForegroundColor DarkBlue
		$web = Get-SPWeb $SiteURL
		$departmentCsvFilePath = get-item $filePath
		Write-Host "Path of file :: " $departmentCsvFilePath
		Write-Host "Reading " $departmentCsvFilePath " file" -ForegroundColor Green

		$derpartmentList = $web.Lists["Departments"]
		foreach($item in Import-Csv $departmentCsvFilePath)
		{
			$isExistedId = CheckIfCodeExist $derpartmentList $item.Code
			Write-Host $item.Code
			if($isExistedId -ne 0)
			{
				Write-Host "Update department: " $item.CommonName
				UpdateDepartment $web $derpartmentList $isExistedId $item
			}
			else
			{
				InsertDepartment $web $derpartmentList $item
	
			}
						
			Write-Host "Please wait ...." -ForegroundColor DarkMagenta
			Start-Sleep -s 15
		}

		#Update upper department
		foreach($item in Import-Csv $departmentCsvFilePath)
		{
			if($item.UpperDepartment -ne "" -AND  $item.UpperDepartment -ne $null )
			{
				$itemIdUpdate = CheckIfCodeExist $derpartmentList $item.Code
					
				if($itemIdUpdate -ne 0)
				{
					UpdateUpperDepartment $web $derpartmentList  $item $itemIdUpdate
				}
			}
		}
		$web.Dispose()
	}
}

function UpdateUpperDepartment($web, $list, $item, $itemIdToUpdate)
{
	try 
		{
		$upperDepartmentId = CheckIfCodeExist $list $item.UpperDepartment

		if($upperDepartmentId -ne 0)
		{
			Write-Host "Update Upper department for: " $item.CommonName

			$itemUpdate = $list.GetItemById($itemIdToUpdate)
			$itemUpdate["UpperDepartment"] = $upperDepartmentId
			$itemUpdate.Update()
			Write-Host "Updated upper department successfully" -ForegroundColor Green
		}
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Update upper department for  $item.CommonName: $_" |Out-File $currentDirectory"\PowerShellLogs\ImportDepartment.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}

function CheckIfCodeExist($list, $code)
{
	$qryCode = new-object Microsoft.SharePoint.SPQuery
	$qryCode.Query = "<Where><Eq> <FieldRef Name='Code' /><Value Type='Text'>"+$code+"</Value></Eq></Where>"
	$dataResults = $list.getItems($qryCode)
	if($dataResults.Count -eq 0)
	{
		return 0
	}
	else
	{
		return $dataResults[0].ID
	}    
}

function UpdateDepartment($web, $list, $itemId, $itemData)
{
	try{

		$itemToUpdate = $list.GetItemById($itemId)

		if($itemData.AdminAd -ne $null -and $itemData.AdminAd -ne "")
		{
			$userAdmin = $web.EnsureUser($itemData.AdminAd)
			if($userAdmin -ne $null)
			{
				$SPFieldUserValueAdmin = New-Object Microsoft.SharePoint.SPFieldUserValue($web, $userAdmin.Id, $userAdmin.Name) 
				$itemToUpdate["Administrator"] = $SPFieldUserValueAdmin
			}
		}

		if($itemData.BODApproverAd -ne "" -and $itemData.BODApproverAd -ne $null)
		{
			$userBODApprover = $web.EnsureUser($itemData.BODApproverAd)
			if($userBODApprover -ne $null)
			{
				$SPFieldUserValueBOD = New-Object Microsoft.SharePoint.SPFieldUserValue($web, $userBODApprover.Id, $userBODApprover.Name) 
				$itemToUpdate["BODApprover"] = $SPFieldUserValueBOD
			}
		}
		
		$factoryLocationList = $web.Lists["Factories"]
		
		$itemToUpdate["CommonName"] = $itemData.CommonName
		$itemToUpdate["DepartmentNo"] = $itemData.DepartmentNo
		$itemToUpdate["CommonName1066"] = $itemData.CommonName1066
		$itemToUpdate["Code"] = $itemData.Code
		$itemToUpdate["SortOrder"] =  $itemData.SortOrder
		$itemToUpdate["CommonMultiLocations"] = Get-GroupLocationString $factoryLocationList $itemData.Locations
		$itemToUpdate["IsShiftRequestRequired"] = $itemData.IsShiftRequestRequired
		$itemToUpdate["IsBODApprovalRequired"] = $itemData.IsBODApprovalRequired		
		$itemToUpdate["IsVisible"] = $itemData.IsVisible
		$itemToUpdate["AutoCreateOverTime"] = $itemData.AutoCreateOverTime

		$itemToUpdate.Update()
		Write-Host "Updated successfully" -ForegroundColor Green
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Update department $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportDepartment.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}

function InsertDepartment($web,$list, $itemData)
{
	try{
		$newItem = $list.Items.Add()

		if($itemData.AdminAd -ne $null -and $itemData.AdminAd -ne "")
		{
			$userAdmin = $web.EnsureUser($itemData.AdminAd)
			if($userAdmin -ne $null)
			{
				$SPFieldUserValueAdmin = New-Object Microsoft.SharePoint.SPFieldUserValue($web, $userAdmin.Id, $userAdmin.Name) 
				$newItem["Administrator"] = $SPFieldUserValueAdmin
			}
		}

		if($itemData.BODApproverAd -ne "" -and $itemData.BODApproverAd -ne $null)
		{
			$userBODApprover = $web.EnsureUser($itemData.BODApproverAd)
			if($userBODApprover -ne $null)
			{
				$SPFieldUserValueBOD = New-Object Microsoft.SharePoint.SPFieldUserValue($web, $userBODApprover.Id, $userBODApprover.Name) 
				$newItem["BODApprover"] = $SPFieldUserValueBOD
			}
		}

		$factoryLocationList = $web.Lists["Factories"]
		
		$newItem["CommonName"] = $itemData.CommonName
		$newItem["DepartmentNo"] = $itemData.DepartmentNo
		$newItem["CommonName1066"] = $itemData.CommonName1066
		$newItem["Code"] = $itemData.Code
		$newItem["SortOrder"] =  $itemData.SortOrder
		$newItem["CommonMultiLocations"] = Get-GroupLocationString $factoryLocationList $itemData.Locations
		$newItem["IsShiftRequestRequired"] = $itemData.IsShiftRequestRequired 	
		$newItem["IsBODApprovalRequired"]=$itemData.IsBODApprovalRequired
		$newItem["IsVisible"]=$itemData.IsVisible
        $newItem["AutoCreateOverTime"]=$itemData.AutoCreateOverTime
		$newItem.Update()
		Write-Host "Inserted successfully" -ForegroundColor Green
		}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Insert department $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportDepartment.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}

function Get-GroupLocationString($factoryLocationList, $locationValueStr)
{
    if($locationValueStr -eq $null -or $locationValueStr -eq "")
    {
        return "";
    }
	if($locationValueStr -Match ";")
	{
		$locationValues = $locationValueStr.Split("{;}")
	}
	else
	{
		$locationValues = @($locationValueStr) #Array 1 element
	}
    
	$itemValues = new-object Microsoft.SharePoint.SPFieldLookupValueCollection

	$locationValues | ForEach {
			$qry = new-object Microsoft.SharePoint.SPQuery
			$qry.Query = "<Where><Eq> <FieldRef Name='CommonName' /><Value Type='Text'>"+$_+"</Value></Eq></Where>"
			$factoryLocationData = $factoryLocationList.getItems($qry)
			#Write-Host $_ -ForegroundColor Green
			if($factoryLocationData.Count -gt 0)
			{
				$lookupValue = New-Object Microsoft.SharePoint.SPFieldLookupValue($factoryLocationData[0].ID,$factoryLocationData[0].ID);
				$itemValues.Add($lookupValue)
			}
	}
	return $itemValues.ToString()
}

Start-Transcript  
Write-Host "START IMRPORT DEPARTMENT DATA" -ForegroundColor Blue
Main $SiteURL $FilePath
Write-Host "WAITING FOR DEPARTMENT..." -ForegroundColor Blue
Start-Sleep -s 30
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell