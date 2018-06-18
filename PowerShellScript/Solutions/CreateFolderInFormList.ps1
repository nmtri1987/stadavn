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

	
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
		Write-Host $SiteURL -ForegroundColor DarkBlue
		$web = Get-SPWeb $SiteURL
		$folderArray = @("Requests","Recruitments","Request for diploma supplies")
		$formList = $web.Lists["Forms"]
		$supportingDocumentList = $web.Lists["Supporting documents"]
		
		$relativeUrl = $web.ServerRelativeUrl
		$folderArray | ForEach {
				 CreateFolder $web $formList $_
				CreateFolder $web $supportingDocumentList $_
		}
		$web.Dispose()
	}
}

function CreateFolder($web, $list, $folderName)
{
	try
	{		
		# Create desired number of subfolders
		$folder = $list.AddItem("", [Microsoft.SharePoint.SPFileSystemObjectType]::Folder, $folderName)
		$folder.Update()
	
		Write-Host "Created successfully" -ForegroundColor Green
	}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Create folder $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\CreateFolderInFormList.txt" -Append;   
		#Write-Host "Error : " $_ -ForegroundColor Red
	}
}

Start-Transcript  
Write-Host "START CREATE FOLDER" -ForegroundColor Blue
Main $SiteURL
Write-Host "DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell