Param($SiteURL, $FilePath);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

Add-PSSnapin Microsoft.Sharepoint.PowerShell
set-executionpolicy remotesigned

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition

if($FilePath -eq $null)
{
	$FilePath = $currentDirectory + "\CSVFile\RecruitmentTeam.csv"
}

if ($SiteURL -eq $null)
{
    $SiteURL = "http://localhost"
}

function Main($SiteURL){
    ImportRecruitmentTeam $SiteURL
}

function ImportRecruitmentTeam($SiteURL)
{
    $web = Get-SPWeb $SiteURL

    $fileExcelPath = get-item $FilePath
    Write-Output "Path of file :: " $fileExcelPath

    $employeeInfo = $web.Lists["Employees"]
    $recruitmentTeamList = $web.GetList("Lists/RecruitmentTeam")
	$recruitmentTeamLookupFieldValue = new-object Microsoft.SharePoint.SPFieldLookupValueCollection
	$title = ""
	$values = "";
    foreach($i in Import-Csv $fileExcelPath)
    {
        $IsCanUpdate = $true
        $employee = $NULL
        $qry = new-object Microsoft.SharePoint.SPQuery
        $qry.Query = " <Where> <Eq> <FieldRef Name='EmployeeID' /><Value Type='Text'>" + $i.EmployeeID + "</Value> </Eq></Where>"
        $employeeInfos = $employeeInfo.GetItems($qry)
         
        if($employeeInfos.Count -gt 0)
        {
            $employee  = $employeeInfos[0]
        }
        else
        {
            #Default is new item
        }
		$title = $title + $employee["EmployeeDisplayName"].ToString() + ";"
		$values += $values + $employee.ID + ";#"
    }
	#
	$recruitmentTeamItem = $recruitmentTeamList.Items.Add()
	$recruitmentTeamItem["Title"] = $title
	$recruitmentTeamItem["Employees"] = $values
	$recruitmentTeamItem.Update()
}

Start-Transcript  

Write-Host "START IMPORT RECRUITMENT TEAM" -ForegroundColor Blue

Main $SiteURL $FilePath

Write-Host "DONE" -ForegroundColor Blue

Stop-Transcript
