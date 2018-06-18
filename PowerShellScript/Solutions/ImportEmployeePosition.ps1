Param($SiteURL,$FilePath);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

if($SiteURL -eq $null){
			$SiteURL="http://localhost"
}
if($FilePath -eq $null)
{
	$FilePath = ".\CSVFile\EmployeePosition.csv"
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

function Main($SiteURL,$FilePath){
		Write-Host $SiteURL
		$web = get-spweb $SiteURL 
		$employeePositionList = $web.Lists["Employee Position"]
		$source = Import-Csv -Delimiter "," -Path $FilePath
        $actionname = "Insert "
	  	
        foreach ($item in $source)
		{
			try{
				$qEmployeePosition = new-object Microsoft.SharePoint.SPQuery
				$qEmployeePosition.Query = "<Where><Eq> <FieldRef Name='ID' /><Value Type='Counter'>"+$item.ID.Trim()+"</Value></Eq></Where>"
				$dataResult = $employeePositionList.getItems($qEmployeePosition)
				#$stringItems = Get-GroupAccessString $groupList  $item.Groups
				if($dataResult.Count -eq 0)
				{
					$newItem = $employeePositionList.AddItem()
					#Write-Host "Created " $item.CommonName -ForegroundColor Green
				}
				else
				{
					$actionname = "Update "
					$newitem = $dataResult[0]
				}

				$newitem["Code"] = $item.Code
				$newitem["CommonName"] = $item.CommonName
				$newitem["CommonName1066"] = $item.CommonNameVietnamese
				$newitem["EmployeeLevel"] = $item.EmployeeLevel

				$newitem.Update()

				Write-Host $actionname $item.CommonName " Successfully " -ForegroundColor Green
			}
			catch
			{
				$currentDate = Get-Date
				"$currentDate : Error item  $item : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportEmployeePosition.txt" -Append;   
				Write-Host "Error : " $item ":" $_ -ForegroundColor Red
			}
		}
        $employeePositionList.Update()
		$web.Dispose()
}

Start-Transcript  
Write-Host "START IMRPORT EMPLOYEE POSITION DATA" -ForegroundColor Blue
Main $SiteURL $FilePath
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell