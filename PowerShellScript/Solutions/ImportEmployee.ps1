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
	$FilePath = $currentDirectory + "\CSVFile\STADA-AllUsersUpdate.csv"
}

if ($SiteURL -eq $null)
{
    $SiteURL = "http://windev16"
}

function Main($SiteURL){
    ImportEmployee $SiteURL
}

function ImportEmployee($SiteURL)
{
    $web = Get-SPWeb $SiteURL

    $fileExcelPath = get-item $FilePath
    Write-Output "Path of file :: " $fileExcelPath

    $employeeInfo= $web.Lists["Employees"]
    $departments = $web.Lists["Departments"]
    $factories = $web.Lists["Factories"]
    $positions = $web.Lists["Employee Position"]

    #Insert Employee Info
    foreach($i in Import-Csv $fileExcelPath)
    {
        $IsCanUpdate = $true
        $newEmployee = $employeeInfo.Items.Add()
        $qry = new-object Microsoft.SharePoint.SPQuery
        $qry.Query = " <Where> <Eq> <FieldRef Name='EmployeeID' /><Value Type='Text'>" + $i.EmployeeID + "</Value> </Eq></Where>"
        $employeeInfos = $employeeInfo.GetItems($qry)
         
        if($employeeInfos.Count -gt 0)
        {
            Write-Host "Starting to updating"
            $newEmployee  = $employeeInfos[0]
        }
        else
        {
            #Default is new item
        }
		try {
			Write-Host "EmployeeID: " $i.EmployeeID
			$newEmployee["FirstName"] = ($i.FirstName + "").Trim()
			$newEmployee["LastName"] = ($i.LastName + "").Trim()
			$newEmployee["EmployeeDisplayName"] =  ($i.LastName + " " +$i.FirstName).Trim()
			$newEmployee["EmployeeType"] = ($i.EmployeeType + "").Trim();
            $newEmployee["DepartmentPermission"] = ($i.EmployeePermission + "").Trim();
			$newEmployee["HomeTown"] = ($i.HomeTown + "").Trim()
			$newEmployee["HomeAddress"] = ($i.Address + "").Trim()
			#$newEmployee["Position"] = 1
			$newEmployee["JoinedDate"] = $i.JoinedDate
			if($i.Email -ne $null -and $i.Email -ne "")
			{
				$newEmployee["Email"] = ($i.Email + "").Trim()
			}
			$newEmployee["EmployeeID"] = ($i.EmployeeID + "").Trim()
			$gender = ($i.Gender + "").Trim()
			
			if($gender -ne $null -and $gender -ne "")
			{
				if($gender -eq "M" -or $gender -eq "Male")
				{
					$newEmployee["Gender"] = "Male"
				}
				else
				{
					$newEmployee["Gender"] = "Female"
				}
			}

			if ($i.JoinedDate -ne $null -and $i.JoinedDate -ne "")
			{
				$newEmployee["JoinedDate"] = [datetime]::ParseExact(($i.JoinedDate + "").Trim(),"M/d/yyyy", $null)
			}

			if ($i.Birthday -ne $null -and $i.Birthday -ne "")
			{
				$newEmployee["Birthday"] = [datetime]::ParseExact(($i.Birthday + "").Trim(),"M/d/yyyy", $null)
			}
			Write-Host "Factory ID: " $i.FactoryLocation
			$query = New-Object  Microsoft.SharePoint.SPQuery 
			$query.query =" <Where> <Eq><FieldRef Name='CommonName' /><Value Type='Text'>" + ($i.FactoryLocation + "").Trim() + "</Value></Eq></Where>" 
			$items = $factories.getItems($query) 
			if ($items.Count -gt 0)
			{
				$newEmployee["CommonLocation"] = $items[0].ID
			}

			Write-Host "Department: " $i.Department
			$query1 = New-Object  Microsoft.SharePoint.SPQuery 
			$query1.query =" <Where><Eq><FieldRef Name='CommonName' /><Value Type='Text'>"+$i.Department+"</Value></Eq></Where>"
			$items1 = $departments.getItems($query1)
			if ($items1.Count -gt 0)
			{
				Write-Host "Department ID: "  $items1[0].ID
				$newEmployee["EmployeeInfoDepartment"] = $items1[0].ID
			}

			Write-Host "Position: " $i.Position
			$query2 = New-Object  Microsoft.SharePoint.SPQuery 
			$query2.query ="<Where><Eq><FieldRef Name='Code' /><Value Type='Text'>"+$i.Position+"</Value></Eq></Where>"
			$items2 = $positions.getItems($query2)
			if ($items2.Count -gt 0)
			{
				Write-Host "Position ID: "  $items2[0].ID
				$newEmployee["Position"] = $items2[0].ID
			}

            $ADAccount = ($i."AD Account" + "").Trim()
            Write-Host "AD Account: " $ADAccount
            if($ADAccount -eq $null)
            {
                Write-Host "Cannot find user " $ADAccount
                $IsCanUpdate = $false
            }
            else
            {
                $user = $web.EnsureUser($ADAccount)
                if ($user -eq $null)
                {
                    Write-Host "Cannot find user " $ADAccount
                    $IsCanUpdate = $false
                }
                else
                {
                    $SPFieldUserValue = New-Object Microsoft.SharePoint.SPFieldUserValue($SPWeb, $user.Id, $user.Name) 
                    $newEmployee["ADAccount"] = $SPFieldUserValue
                }
            }
            if ($IsCanUpdate)        
            {
                $newEmployee.Update()
                Write-Host -ForegroundColor Green "Save employee successfully!"
            }
         }
        catch {
			Write-Host -ForegroundColor Red "Error: Cannot find user " $i."AD Account"
			$currentDate = Get-Date
			"$currentDate : Error - Cannot find user " + $i."AD Account" |Out-File $currentDirectory"\PowerShellLogs\ImportEmployee.txt" -Append;   
            $IsCanUpdate = $false
        }
    } 
   

    #update Manager 
    foreach($i in Import-Csv $fileExcelPath)
    {
        $query5 = New-Object  Microsoft.SharePoint.SPQuery 
        $query5.query = "<Where><Eq> <FieldRef Name='EmployeeID' /><Value Type='Text'>" + $i.EmployeeID + "</Value> </Eq></Where>" 
        $employee = $employeeInfo.getItems($query5)
        $updateEmp = $employee[0]

        try
        {
            if ($i.Manager -eq "")
            {
                #Don't have manager
            }
            else
            { 
                Write-Host "Update Manager: " $i.Manager " -> " $i.FirstName + $i.LastName
                $query = New-Object  Microsoft.SharePoint.SPQuery 
                $query.query = "<Where><Eq><FieldRef Name='EmployeeID' /><Value Type='Text'>" + $i.Manager + "</Value></Eq></Where>"
                $managerID = $employeeInfo.getItems($query)
                if ($managerID.Count -gt 0)
                {
                    $updateEmp["EmployeeInfoManager"] = New-Object Microsoft.SharePoint.SPFieldLookupValue($managerID[0].ID, $managerID[0].ID.ToString())
                    $updateEmp.Update()
                }
                else
                {
                    Write-Host -ForegroundColor Yellow "Cannot find manager " $i.Manager
                }
            }
        }
        catch
        {
			$currentDate = Get-Date
			"$currentDate : Error - Cannot find user " + $i."AD Account" |Out-File $currentDirectory"\PowerShellLogs\ImportEmployee.txt" -Append;   
            Write-Host "Something went wrong"
        }
       
    }
}

Start-Transcript  

Write-Host "START IMPORT EMPLOYEES" -ForegroundColor Blue

Main $SiteURL $FilePath

Write-Host "DONE" -ForegroundColor Blue

Stop-Transcript
