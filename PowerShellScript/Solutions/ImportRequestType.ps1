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
			$FilePath = $currentDirectory  + "\CSVFile\RequestTypeData.csv"
		}

		Write-Host $SiteURL
		$web = get-spweb $SiteURL 
		$requestTypeList = $web.Lists["Request Types"]
		$departmentList = $web.Lists["Departments"]

		#$requestTypeCsvFilePath = get-item $FilePath
        $actionname = "Insert "
	  	try{
				foreach ($item in  Import-Csv $FilePath)
				{
					$requestTypeQuery = new-object Microsoft.SharePoint.SPQuery
					$requestTypeQuery.Query = "<Where><Eq> <FieldRef Name='RequestTypeName' /><Value Type='Text'>"+$item.RequestTypeName.Trim()+"</Value></Eq></Where>"
					$dataResult = $requestTypeList.getItems($requestTypeQuery)
					if($dataResult.Count -eq 0)
					{
						$newItem = $requestTypeList.AddItem()
					}
					else
					{
						$actionname = "Update "
						$newitem = $dataResult[0]
					}
					$newitem["RequestTypeName"] = $item.RequestTypeName
					$newitem["RequestType"] = $item.RequestType

					$newItem["Departments"] = Get-GroupDepartmentString $departmentList $item.Departments
					$newitem.Update()

					Write-Host $actionname $item.RequestTypeName " Successfully " -ForegroundColor Green
				}
				$requestTypeList.Update()
			  }
		catch
        {
			$currentDate = Get-Date
			"$currentDate : Error $_ " |Out-File $currentDirectory"\PowerShellLogs\ImportRequestType.txt" -Append;   
			Write-Host "Error: " $_ -ForegroundColor Red
        }
		$web.Dispose()	
}

function Get-GroupDepartmentString($departmentList, $departmentString)
{
    if($departmentString -eq $null -or $departmentString -eq "")
    {
        return "";
    }
	if($departmentString -Match ";")
	{
		$departments = $departmentString.Split("{;}")
	}
	else
	{
		$departments = @($departmentString) #Array 1 element
	}
    
	$itemValues = new-object Microsoft.SharePoint.SPFieldLookupValueCollection

	$departments | ForEach {
			$qry = new-object Microsoft.SharePoint.SPQuery
			$qry.Query = "<Where><Eq> <FieldRef Name='CommonName' /><Value Type='Text'>"+$_+"</Value></Eq></Where>"
			$departmentData = $departmentList.getItems($qry)
			#Write-Host $_ -ForegroundColor Green
			if($departmentData.Count -gt 0)
			{
				$lookupValue = New-Object Microsoft.SharePoint.SPFieldLookupValue($departmentData[0].ID,$departmentData[0].ID);
				$itemValues.Add($lookupValue)
			}
	}
	return $itemValues.ToString()
}



Start-Transcript  
Write-Host "START IMRPORT REQUEST TYPE DATA" -ForegroundColor Blue
Main $SiteURL $FilePath
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell