Param($SiteURL);
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SharePoint")
if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

if($SiteURL -eq $null){
	$SiteURL="http://windev162:1111"
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

#$csvFilePath = $currentDirectory + "\CSVFile\FileNameToUploadToDocumentList.csv"
$uploadFilesPath = $currentDirectory + "\EmployeeImages"

function Main($SiteURL)
{
	#Document Library where you want to upload files
	#$libraryName = "SiteCollectionImages"
    $libraryName = "EmployeeImages"
	#Physical/Network location of files
	$reportFilesLocation  = $uploadFilesPath

    $libraryTemplateDL = [Microsoft.SharePoint.SPListTemplateType]::PictureLibrary

	$spWeb = Get-SPWeb $SiteURL;
    $spListCollection = $spWeb.Lists    
    $spLibrary = $spListCollection.TryGetList($libraryName) 
    if($spLibrary -ne $null) {
        Write-Host -f Yellow "Library $LibraryName already exists in the site"
    }
    else {
        Write-Host -NoNewLine -f yellow "Creating  Library - $libraryName"   
        $spListCollection.Add($libraryName, $libraryName, $libraryTemplateDL)    
        $spLibrary = $spWeb.GetList($spWeb.ServerRelativeUrl + $libraryName) 
        Write-Host -f Green "...Success!"    
    }


	#$listUrl = $SiteURL +"/"+$libraryName
	
	#$spSourceList = $spSourceWeb.GetList($listUrl)

	$employeeImages = ([System.IO.DirectoryInfo] (Get-Item $reportFilesLocation)).GetFiles()
	foreach($employeeImage in $employeeImages)
	{
		try{
			#Open file
			$fileStream = ([System.IO.FileInfo] (Get-Item $employeeImage.FullName)).OpenRead()

			#Add file
			$folder =  $spWeb.getfolder($libraryName)

			Write-Host "> Copying file $employeeImage to $libraryName..."
			$spFile = $folder.Files.Add($folder.Url + "/" + $employeeImage.Name, [System.IO.Stream]$fileStream, $true)

			#Close file stream
			$fileStream.Close();
			
			#update user profile
            $fileName = $employeeImage.Name
            [string[]]$arr = $fileName.Split('.',[System.StringSplitOptions]::RemoveEmptyEntries)
			$employeeId =  $arr[0]
			$userImageUrl = "/" + $libraryName + "/" + $fileName
			Update-Employee-Image $employeeId  $userImageUrl $spWeb
			
		Write-Host " "
		}
		catch
		{
			$currentDate = Get-Date
			"$currentDate : Error - Upload file $employeeImage : $_" |Out-File $currentDirectory"\PowerShellLogs\UploadEmployeeImagesToImagesList.txt" -Append;   
			Write-Host "Error : " $_ -ForegroundColor Red
		}
	}

	$spWeb.dispose();
	Write-Host "Files have been uploaded to $libraryName." -ForegroundColor Green
}
function Update-Employee-Image($employeeId, $userImageUrl, $web)
{
        Write-Host "Updating Employee Images with Id"  $employeeId  "..." 
        $employeeInfo= $web.Lists["Employees"]
        $qry = new-object Microsoft.SharePoint.SPQuery
        $qry.Query = "<Where><Eq><FieldRef Name='EmployeeID'/><Value Type='Text'>" + $employeeId + "</Value></Eq></Where>"
        $employees = $employeeInfo.GetItems($qry)
        if($employees.Count -gt 0)
        {
              
           $employee = $employees[0]
		   $imgTemplate = "<img src='{0}' width='100' ></img>" -f $userImageUrl
		   $employee["PublishingPageImage"]= $imgTemplate
		   $employee.update()
        }

}
Start-Transcript  
Write-Host "START UPLOAD FILE TO DOCUMENT LIST" -ForegroundColor Blue
Main $SiteURL 
Write-Host "UPLOAD DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell