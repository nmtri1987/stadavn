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
			$FilePath = $currentDirectory  + "\CSVFile\ForeignLanguageLevelData.csv"
		}

		Write-Host $SiteURL
		$web = get-spweb $SiteURL 
		$foreignLanguageLevelList = $web.Lists["Foreign language levels"]
		
		#$requestTypeCsvFilePath = get-item $FilePath
        $actionname = "Insert "
	  	try{
				foreach ($item in  Import-Csv $FilePath)
				{
					$foreignLanguageLevelListQuery = new-object Microsoft.SharePoint.SPQuery
					$foreignLanguageLevelListQuery.Query = "<Where><Eq> <FieldRef Name='Title' /><Value Type='Text'>"+$item.Title.Trim()+"</Value></Eq></Where>"
					$dataResult = $foreignLanguageLevelList.getItems($foreignLanguageLevelListQuery)
					if($dataResult.Count -eq 0)
					{
						$newItem = $foreignLanguageLevelList.AddItem()
					}
					else
					{
						$actionname = "Update "
						$newitem = $dataResult[0]
					}
					$newitem["Title"] = $item.Title.Trim()
					$newitem.Update()

					Write-Host $actionname $item.Title " successfully " -ForegroundColor Green
				}
				$foreignLanguageLevelList.Update()
			  }
		catch
        {
			$currentDate = Get-Date
			"$currentDate : Error $_ " |Out-File $currentDirectory"\PowerShellLogs\ImportForeignLanguageLevel.txt" -Append;   
			Write-Host "Error: " $_ -ForegroundColor Red
        }
		$web.Dispose()	
}

Start-Transcript  
Write-Host "START IMRPORT FOREIGN LANGUAGE LEVEL DATA" -ForegroundColor Blue
Main $SiteURL $FilePath
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell