Param($SiteURL);
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SharePoint")
if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

if($SiteURL -eq $null){
	$SiteURL="http://localhost"
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

$csvFilePath = $currentDirectory + "\CSVFile\FileNameToUploadToDocumentList.csv"
$uploadFilesPath = $currentDirectory + "\FileUpload"

function Main($SiteURL)
{
	#Document Library where you want to upload files
	$libraryName = "Shared Documents"
	#Physical/Network location of files
	$reportFilesLocation  = $uploadFilesPath

	$spSourceWeb = Get-SPWeb $SiteURL;
	$listUrl = $SiteURL +"/"+$libraryName
	
	$spSourceList = $spSourceWeb.GetList($listUrl)

	if($spSourceList -eq $null)
	{
		Write-Host "The Library $libraryName could not be found."
		return;
	}

	$files = ([System.IO.DirectoryInfo] (Get-Item $reportFilesLocation)).GetFiles()
	foreach($file in $files)
	{
		try{
			#Open file
			$fileStream = ([System.IO.FileInfo] (Get-Item $file.FullName)).OpenRead()

			#Add file
			$folder =  $spSourceWeb.getfolder($libraryName)

			Write-Host "> Copying file $file to $libraryName..."
			$spFile = $folder.Files.Add($folder.Url + "/" + $file.Name, [System.IO.Stream]$fileStream, $true)

			#Close file stream
			$fileStream.Close();
		Write-Host " "
		}
		catch
		{
			$currentDate = Get-Date
			"$currentDate : Error - Upload file $file : $_" |Out-File $currentDirectory"\PowerShellLogs\UploadFileToDocumentList.txt" -Append;   
			Write-Host "Error : " $_ -ForegroundColor Red
		}
	}
	$spSourceWeb.dispose();
	Write-Host "Files have been uploaded to $libraryName." -ForegroundColor Green
}

Start-Transcript  
Write-Host "START UPLOAD FILE TO DOCUMENT LIST" -ForegroundColor Blue
Main $SiteURL 
Write-Host "UPLOAD DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell