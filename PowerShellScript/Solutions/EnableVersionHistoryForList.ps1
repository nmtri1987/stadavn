Param($SiteURL);
if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
   Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}
set-executionpolicy remotesigned

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main($SiteURL)
{
	Write-Host $SiteURL -ForegroundColor DarkGreen
	$filePath = ".\CSVFile\EnableVersionHistoryForList.csv"
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		$web = Get-SPWeb $SiteURL
		$versionHistoryCsvFilePath = get-item $filePath
		Write-Host "Path of file :: " $versionHistoryCsvFilePath
		Write-Host "Reading " $versionHistoryCsvFilePath " file" -ForegroundColor Green

		foreach($item in Import-Csv $versionHistoryCsvFilePath)
		{
			try{
				Write-Host $item.ListName
				$list = $web.Lists[$item.ListName]
				#$list.AllowUnsafeUpdates  = $true	

				$list.EnableVersioning = $true
				#$list.EnableMinorVersions = $true
				$list.MajorVersionLimit = 10
				#$list.MajorWithMinorVersionsLimit = 5
				$list.Update()
				Write-Host "=> Enabled successfully" -ForegroundColor Green
			}
			catch
			{
				$currentDate = Get-Date
				"$currentDate : Error - Enable version history $item.ListName : $_" |Out-File $currentDirectory"\PowerShellLogs\EnableVersionHistoryForList.txt" -Append;   
				Write-Host "Error - Enable version history $item.ListName : $_" -ForegroundColor Red
			}
		}
	}
}

Start-Transcript  
Write-Host "START ENABLE VERSION HISTORY" -ForegroundColor Blue
Main $SiteURL 
Write-Host "DONE" -ForegroundColor Blue
Stop-Transcript
