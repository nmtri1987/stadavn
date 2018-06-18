Param($SiteURL);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

if($SiteURL -eq $null){
	$SiteURL="http://localhost"
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function UpdateConfigurations($SiteURL)
{
    $filePath = $currentDirectory + "\CSVFile\Configurations.csv"
		
	if ($SiteURL -eq $null)
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		Write-Host $SiteURL -ForegroundColor DarkBlue
		$web = Get-SPWeb $SiteURL
		$configurations = get-item $filePath
		Write-Host "Path of file :: " $filePath
		Write-Host "Reading " $filePath " file" -ForegroundColor Green
		$configurationsList = $web.Lists["Configurations"]

		foreach($item in Import-Csv $configurations)
		{
            $spListItem = GetSPListItemByKey $configurationsList $item.Key
			if($spListItem -ne $null)
			{
				Write-Host "Updating Key: " $item.Key
				$spListItem["Title"] = $item.Title
                $spListItem["Value"] = $item.Value
                $spListItem.Update()
			}
            else
            {
                Write-Host "Adding Key: " $item.Key
                $spListItem = $configurationsList.Items.Add()
                $spListItem["Title"] = $item.Title
                $spListItem["Key"] = $item.Key
                $spListItem["Value"] = $item.Value
                $spListItem.Update()
            }
		}

		$web.Dispose()
	}
}

function GetSPListItemByKey($configurationsList, $key)
{
	$qryKey = new-object Microsoft.SharePoint.SPQuery
	$qryKey.Query = "<Where><Eq> <FieldRef Name='Key' /><Value Type='Text'>"+$key+"</Value></Eq></Where>"
	$items = $configurationsList.getItems($qryKey)
	if($items.Count -eq 0)
	{
		return $null
	}
	else
	{
		return $items[0]
	}    
}

Start-Transcript  
Write-Host "START UPDATE CONFIGURATIONS" -ForegroundColor Blue
UpdateConfigurations $SiteURL
Write-Host "UPDATE DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell