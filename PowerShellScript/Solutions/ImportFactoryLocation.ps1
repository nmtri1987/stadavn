Param($SiteURL,$FilePath);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

function Main($SiteURL,$FilePath){

		if($SiteURL -eq $null){
			$SiteURL="http://localhost"
		}
		if($FilePath -eq $null)
		{
			$FilePath = ".\CSVFile\FactoryLocation.csv"
		}

		Write-Host $SiteURL
		$web = get-spweb $SiteURL 
		$factoryLocationList = $web.Lists["Factories"]
		$source = Import-Csv -Delimiter "," -Path $FilePath
        $actionname = "Insert "
	  	        foreach ($item in $source)
		{
            $qFactoryLocation = new-object Microsoft.SharePoint.SPQuery
			$qFactoryLocation.Query = "<Where><Eq> <FieldRef Name='ID' /><Value Type='Counter'>"+$item.ID.Trim()+"</Value></Eq></Where>"
			$dataResult = $factoryLocationList.getItems($qFactoryLocation)
            #$stringItems = Get-GroupAccessString $groupList  $item.Groups
            if($dataResult.Count -eq 0)
            {
			    $newItem = $factoryLocationList.AddItem()
                #Write-Host "Created " $item.CommonName -ForegroundColor Green
            }
            else
            {
                $actionname = "Update "
                $newitem = $dataResult[0]
            }

            #$newitem["ID"] = $item.ID
            #$newitem["Name"] = $item.Code
            $newitem["CommonName"] = $item.Name

            $newitem.Update()

            Write-Host $actionname $item.Name " Successfully " -ForegroundColor Green
		}
        $factoryLocationList.Update()
		$web.Dispose()
	
}


Start-Transcript  
Write-Host "START IMRPORT Factory Location DATA" -ForegroundColor Blue
Main $SiteURL $FilePath
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell