Param($SiteURL);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

if($SiteURL -eq $null){
	$SiteURL="http://localhost:81"
}

$currentDirectory = split-path -parent $MyInvocation.MyCommand.Definition


function Main($SiteURL)
{

	
	if ($SiteURL -eq $null )
	{
	   Write-Host "Cannot find Site" -ForegroundColor Red
	}
	else
	{
        
        $web = Get-SPWeb $SiteURL

         $employeeInfo= $web.Lists["Employees"]
         $shiftManagementList = $web.Lists["Shift Management"] 
         $shiftManagementDetailList = $web.Lists["Shift Management Detail"] 
            $departmentsList = $web.Lists["Departments"]
            $shiftTimeList = $web.Lists["Shift Time"]

        $RequesterID = Read-Host -Prompt 'Input requesterID (Ex: 10060) '
        $ApprovedBy = Read-Host -Prompt 'Input Approved By (Ex: minhit) '
        $Month = Read-Host -Prompt 'Input month '
        $Year = Read-Host -Prompt 'Input year '
        $DepartmentCode = Read-Host -Prompt 'Input department code (Ex: IT) '
        $LocationID = Read-Host -Prompt 'Input Location Id (Ex: 2) '
        $ShiftTimeCode = Read-Host -Prompt 'Input Shift Time Code (Ex: HC) '


        Write-Host "Get requester ...."
        $query1 = New-Object  Microsoft.SharePoint.SPQuery 
        $query1.query = "<Where><Eq><FieldRef Name='EmployeeID' /><Value Type='Text'>" + $RequesterID + "</Value></Eq></Where>" 
        $requesterRes = $employeeInfo.getItems($query1)
        $requester = $requesterRes[0]


        Write-Host "Get department ...."
        $query2 = New-Object  Microsoft.SharePoint.SPQuery 
        $query2.query = "<Where><Eq><FieldRef Name='Code' /><Value Type='Text'>" + $DepartmentCode + "</Value></Eq></Where>" 
        $departmentRes = $departmentsList.getItems($query2)
        $department = $departmentRes[0]

        Write-Host "Get Shift time ...."
          $query4 = New-Object  Microsoft.SharePoint.SPQuery 
        $query4.query = "<Where><Eq><FieldRef Name='Code' /><Value Type='Text'>" + $ShiftTimeCode + "</Value></Eq></Where>" 
        $shiftTimeRes = $shiftTimeList.getItems($query4)
        $shiftTime = $shiftTimeRes[0]


        if($requester -ne $null -and $department -ne $null -and $shiftTime -ne $null)
        {
            Write-Host "Create shift management"
            $newItemId = InsertShiftManagement $web $shiftManagementList $requester.ID $Month $Year $department.ID $ApprovedBy $LocationID
            if($newItemId -gt 0)
            {
                Write-Host "Get department all users in department: " $department.CommonName
                
                 $query3 = New-Object  Microsoft.SharePoint.SPQuery 
                 $query3.query = "<Where><Eq><FieldRef Name='EmployeeInfoDepartment' LookupId='TRUE' /><Value Type='Lookup'>" +  $department.ID  + "</Value></Eq></Where>" 
                 $employeesRes = $employeeInfo.getItems($query3)
                 if($employeesRes.Count -gt 0)
                 {
                       foreach($item in $employeesRes)
                       {
                            InsertShiftDetail $web $shiftManagementDetailList $item.ID $shiftTime.ID $newItemId
                       }  
                 }
                 else
                 {
                     Write-Host "Failed, employee user list empty"
                 }
            }
            else
            {
                Write-Host "Failed, please try again"
            }
        }
        $web.Dispose()
	}
}

function InsertShiftDetail ($web,$list, $employeeId, $shiftTimeId, $shiftManagementID)
{
      try{
		$newItem = $list.Items.Add()
		$newItem["Employee"] = $employeeId
        $newItem["ShiftManagementID"] = $shiftManagementID

		for($i=1 
        $i -le 28
        $i++){
             $columnName = "ShiftTime" + $i
             $columnName1 = "ShiftTime" + $i + "Approval"
            
             $newItem[$columnName] = $shiftTimeId
             $newItem[$columnName1] = "False"
         }
		    $newItem.Update()
		    Write-Host "Inserted shift detail successfully" -ForegroundColor Green
            return $newItem.ID
		}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Insert department $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportShiftData.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}


}


function InsertShiftManagement($web,$list, $requesterId, $month, $year, $departmentId, $approvedBy, $locationID)
{
	try{
		$newItem = $list.Items.Add()
		$newItem["Requester"] = $requesterId
		$newItem["CommonMonth"] = $month
		$newItem["CommonYear"] = $year
		$newItem["CommonDepartment"] = $departmentId
        $newItem["CommonLocation"] = $locationID
	
          if($approvedBy -ne $null -and $approvedBy -ne "")
		{
			$userAdmin = $web.EnsureUser($approvedBy)
			if($userAdmin -ne $null)
			{
				$SPFieldUserValueAdmin = New-Object Microsoft.SharePoint.SPFieldUserValue($web, $userAdmin.Id, $userAdmin.Name) 
				$newItem["CommonApprover1"] = $SPFieldUserValueAdmin
			}
		}
		$newItem.Update()
		Write-Host "Inserted successfully" -ForegroundColor Green
    return $newItem.ID
		}
	catch
	{
		$currentDate = Get-Date
		"$currentDate : Error - Insert department $itemData : $_" |Out-File $currentDirectory"\PowerShellLogs\ImportShiftData.txt" -Append;   
		Write-Host "Error : " $_ -ForegroundColor Red
	}
}

Start-Transcript  
Write-Host "START IMRPORT SHIFT DATA" -ForegroundColor Blue
Main $SiteURL
Write-Host "START IMRPORT SHIFT DATA" -ForegroundColor Blue
Write-Host "IMPORT DONE" -ForegroundColor Blue
Stop-Transcript
Remove-PsSnapin Microsoft.SharePoint.PowerShell