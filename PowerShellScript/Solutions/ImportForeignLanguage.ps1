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
			$FilePath = $currentDirectory  + "\CSVFile\ForeignLanguageData.csv"
		}

		Write-Host $SiteURL
		$web = get-spweb $SiteURL 
		$foreignLanguageList = $web.Lists["Foreign Languages"]
		
		#$requestTypeCsvFilePath = get-item $FilePath
        $actionname = "Insert "
	  	try{
				foreach ($item in  Import-Csv $FilePath)
				{
					$foreignLanguageListQuery = new-object Microsoft.SharePoint.SPQuery
					$foreignLanguageListQuery.Query = "<Where><Eq> <FieldRef Name='CommonName' /><Value Type='Text'>"+$item.CommonName.Trim()+"</Value></Eq></Where>"
					$dataResult = $foreignLanguageList.getItems($foreignLanguageListQuery)
					if($dataResult.Count -eq 0)
					{
						$newItem = $foreignLanguageList.AddItem()
					}
					else
					{
						$actionname = "Update "
						$newitem = $dataResult[0]
					}
					$newitem["CommonName"] = $item.CommonName.Trim()
					$newitem["CommonName1066"] = $item.CommonName1066.Trim()
					$newitem.Update()

					Write-Host $actionname $item.Title " successfully " -ForegroundColor Green
				}
				$foreignLanguageList.Update()
			  }
		catch
        {
			$currentDate = Get-Date
			"$currentDate : Error $_ " |Out-File $currentDirectory"\PowerShellLogs\ImportForeignLanguage.txt" -Append;   
			Write-Host "Error: " $_ -ForegroundColor Red
        }
		$web.Dispose()	
}

Start-Transcript  
Write-Host "START IMRPORT FOREIGN LANGUAGE DATA" -ForegroundColor Blue
Main $SiteURL $FilePath
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell