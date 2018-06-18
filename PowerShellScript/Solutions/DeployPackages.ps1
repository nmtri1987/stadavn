Param($SiteURL);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
            Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}

 
function WaitForInsallation([string] $Name)
{
        Write-Host -NoNewline "Waiting for deployment job to complete" $Name "."
        $wspSol = get-SpSolution $Name
        while($wspSol.JobExists)
        {
            sleep 2
            Write-Host -NoNewline "."
            $wspSol = get-SpSolution $Name 
        }
        Write-Host "Job Ended" -ForegroundColor green
}
 
Function Deploy-SPSolution ($WspFolderPath, $SiteURL)
{
	Write-Host $WspFolderPath
    $wspFiles = get-childitem $WspFolderPath | where {$_.Name -like "*.wsp"} #sort CreationTime
 
    ForEach($file in $wspFiles)
    {
        $wsp = Get-SPSolution | Where{$_.Name -eq $file.Name}
        if($wsp -eq $null)
        {
            write-host "Adding solution" -ForegroundColor green
            Add-SPSolution -LiteralPath ($WspFolderPath + "\" + $file.Name)
        }
        else
        {
            write-host "Solution already exists"
             
            if($wsp.Deployed -eq $true)
            {
                write-host "Solution is deployed already, updating the solution"
				if($($wsp.ContainsGlobalAssembly)){
					Write-Host "GAC: "$($wsp.ContainsGlobalAssembly)
					Update-SPSolution -identity $wsp.SolutionId -LiteralPath ($WspFolderPath + "\" + $file.Name) -GACDeployment:$($wsp.ContainsGlobalAssembly) -ErrorAction SilentlyContinue
				}
				else{
					Update-SPSolution -identity $wsp.SolutionId -LiteralPath ($WspFolderPath + "\" + $file.Name) -FullTrustBinDeployment -ErrorAction SilentlyContinue
				}
            }
             else
            {
               write-host "Removing solution" -ForegroundColor Magenta
               Remove-SPSolution -identity $wsp.SolutionId -confirm:$false
                 
                write-host "Adding solution" -ForegroundColor green         
               Add-SPSolution -LiteralPath ($WspFolderPath + "\" + $file.Name)
            }
            WaitForInsallation -Name $wsp.Name
            Write-Host " "
        }
         
     
        $wsp = Get-SPSolution | Where {$_.Name -eq $file.Name}
		$DeployedWebapps = $sol.DeployedWebApplications | where {$_.Url -like $SiteURL }
        if($wsp -ne $null)
        {
            write-host "Installing solution" -ForegroundColor green 

		
			$webApp=Get-SPWebApplication -identity $SiteURL
			
			if($DeployedWebapps -eq $null) {
				Write-host "Deploy Web App Solution" $wsp.Name 
				#Change the following line of code according to the targetted settings of wsp
				if ($wsp.ContainsWebApplicationResource) {
					$wsp | Install-SPSolution -WebApplication $webApp.Url -FullTrustBinDeployment -GACDeployment:$($wsp.ContainsGlobalAssembly) -Force -ErrorAction SilentlyContinue
				}
				else {
					$wsp | Install-SPSolution  -FullTrustBinDeployment -GACDeployment:$($wsp.ContainsGlobalAssembly) -Force -ErrorAction SilentlyContinue
				}
			}
			else {
				Write-host "Update Solution" $wsp.Name -ForegroundColor Yellow 
				if($($wsp.ContainsGlobalAssembly)) {
					Update-SPSolution -identity $wsp.SolutionId  -LiteralPath ($WspFolderPath + "\" + $file.Name) -GACDeployment:$($wsp.ContainsGlobalAssembly) 
				}
				else {
					Update-SPSolution -identity $wsp.SolutionId -LiteralPath ($WspFolderPath + "\" + $file.Name) -FullTrustBinDeployment
				}
			}
				
        }
        WaitForInsallation -Name $wsp.Name
        Write-Host " "
    }
    Write-Host "-- Done --" -ForegroundColor DarkGreen
}
try
{
    	Write-Host  "URL Address: " $SiteURL -ForegroundColor Blue
	$WspFolderPath = ($(get-location).Path).Trim() + "\Packages"
        Deploy-SPSolution -WspFolderPath $WspFolderPath -SiteURL $SiteURL
}
catch
{
    Write-Host $_.exception   
}