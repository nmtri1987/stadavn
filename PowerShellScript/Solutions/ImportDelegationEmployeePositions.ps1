Param($SiteURL);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

if($SiteURL -eq $null){
	$SiteURL="http://localhost"
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main($SiteURL){

	if($FilePath -eq $null)
	{
		$FilePath = $currentDirectory + "\CSVFile\DelegationEmployeePosition.csv"
	}
		
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		Write-Host $SiteURL -ForegroundColor DarkBlue
		$web = Get-SPWeb $SiteURL
		$csvFilePath = get-item $filePath
		Write-Host "Reading " $csvFilePath " file" -ForegroundColor Green

		$delegationEmployeePositionList = $web.Lists["Delegation employee positions"]
		foreach($item in Import-Csv $csvFilePath)
		{
			$isExistedId = CheckIfTitleExist $delegationEmployeePositionList $item.Title
			Write-Host $item.Title
			if($isExistedId -ne 0)
			{
				Write-Host "Update data: " $item.Title
				UpdateDelegationEmployeePosition $web $delegationEmployeePositionList $isExistedId $item
			}
			else
			{
                Write-Host "Insert data: " $item.Title
				InsertDelegationEmployeePosition $web $delegationEmployeePositionList $item
			}
		}
		$web.Dispose()
	}
}

function CheckIfTitleExist($list, $title)
{
	$qryCode = new-object Microsoft.SharePoint.SPQuery
	$qryCode.Query = "<Where><Eq> <FieldRef Name='Title' /><Value Type='Text'>"+$title+"</Value></Eq></Where>"
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

function UpdateDelegationEmployeePosition($web, $list, $itemId, $itemData)
{
	try{
        $positionList = $web.Lists["Employee Position"]
		$itemToUpdate = $list.GetItemById($itemId)
		$itemToUpdate["EmployeePosition"] = Get-GroupPositionString $positionList $itemData.EmployeePosition
		$itemToUpdate["DelegatedEmployeePositions"] = Get-GroupPositionString $positionList $itemData.DelegatedEmployeePositions

		$itemToUpdate.Update()
		Write-Host "Updated successfully" -ForegroundColor Green
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Update delegation employee position $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportDelegationEmployeePositions.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}

function InsertDelegationEmployeePosition($web, $list, $itemData)
{
	try{
		$positionList = $web.Lists["Employee Position"]
		$newItem = $list.Items.Add()
		$newItem["Title"] = $itemData.Title
		$newItem["EmployeePosition"] = Get-GroupPositionString $positionList $itemData.EmployeePosition
		$newItem["DelegatedEmployeePositions"] = Get-GroupPositionString $positionList $itemData.DelegatedEmployeePositions
		$newItem.Update()
		Write-Host "Inserted successfully" -ForegroundColor Green
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Insert delegation employee position $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportDelegationEmployeePositions.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}


function Get-GroupPositionString($positionList, $positionString)
{
    if($positionString -eq $null -or $positionString -eq "")
    {
        return "";
    }
	if($positionString -Match ";")
	{
		$positions = $positionString.Split("{;}")
	}
	else
	{
		$positions = @($positionString) #Array 1 element
	}
    
	$itemValues = new-object Microsoft.SharePoint.SPFieldLookupValueCollection

	$positions | ForEach {
			$qry = new-object Microsoft.SharePoint.SPQuery
			$qry.Query = "<Where><Eq> <FieldRef Name='Code' /><Value Type='Text'>"+$_+"</Value></Eq></Where>"
			$positionData = $positionList.getItems($qry)
			#Write-Host $_ -ForegroundColor Green
			if($positionData.Count -gt 0)
			{
				$lookupValue = New-Object Microsoft.SharePoint.SPFieldLookupValue($positionData[0].ID,$positionData[0].ID);
				$itemValues.Add($lookupValue)
			}
	}
	return $itemValues.ToString()
}


Start-Transcript  
Write-Host "START IMPORT DELEGATION EMPLOYEE POSITION DATA" -ForegroundColor Blue
Main $SiteURL
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell