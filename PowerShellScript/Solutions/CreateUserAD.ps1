Param($filepath, $siteUrl) 

Add-PSSnapin Microsoft.Sharepoint.Powershell
set-executionpolicy remotesigned

if ($filepath -eq $null )
{
    $filepath = "ImportEmployee\STADA-CommonUsers.csv"
    $siteUrl = "http://windev162:1111"
}

function Main()
{
    $Users = Import-Csv -Delimiter "," -Path $filepath           
    foreach ($User in $Users)            
    {            
        $Displayname = $User."Display Name"#$User.Firstname + " " + $User.Lastname            
        $UserFirstname = $User.Firstname            
        $UserLastname = $User.Lastname 
        $Email = $User.Email
        $Email2 = $User.Email2
        $OU = $User."AD Account"            
        $SAM = $User."AD Account" 
        $UPN = $User.Email
        $Password = "RBVH@1234"
		$Department = $User.Department
		$Title =$User."Position(Job Title)"
		$Manager = $User.Manager
		$Address =$User.Address
		$City =$User.HomeTown
		$EmployeeID = $User.EmployeeID
        $UserFound = $null
        try
        {
            $UserFound = Get-ADUser $SAM
        }
        catch
        {
            $UserFound = $null
        }

        Write-Host $UserFound $SAM
        if ($UserFound -eq $null)
        {
            if($Email -eq $null)
            {
                New-ADUser -Name "$Displayname" -GivenName "$UserFirstname" -Surname "$UserLastname" -DisplayName "$Displayname" -SamAccountName $SAM -UserPrincipalName $UPN -AccountPassword (ConvertTo-SecureString $Password -AsPlainText -Force) -Enabled $true -ChangePasswordAtLogon $false –PasswordNeverExpires $true -Department $Department -Title $Title  -StreetAddress $Address -City $City -employeeID  $EmployeeID
            }
            else
            {
                New-ADUser -Name "$Displayname" -GivenName "$UserFirstname" -Surname "$UserLastname" -DisplayName "$Displayname" -EmailAddress $Email -SamAccountName $SAM -UserPrincipalName $UPN -AccountPassword (ConvertTo-SecureString $Password -AsPlainText -Force) -Enabled $true -ChangePasswordAtLogon $false –PasswordNeverExpires $true -Department $Department -Title $Title  -StreetAddress $Address -City $City -employeeID  $EmployeeID
			}

            Write-Host -ForegroundColor Green "Created User" $SAM $Email
        }
        else
        {
            Write-Host -ForegroundColor Yellow $SAM " is existed!"
            #Remove-ADUser -Identity $SAM -Confirm:$false
        }

        SetEmail $siteUrl $SAM $Email2
    
    }
}

function SetEmail($siteUrl, $SAM, $EmailAddress)
{
    $web = get-spweb $siteUrl 
    $user = $web.EnsureUser($SAM) 
    Set-SPUser -Identity $user -Email $EmailAddress -Web $siteUrl
    Write-Host -ForegroundColor Green $EmailAddress " is updated!"
}

Main
pause