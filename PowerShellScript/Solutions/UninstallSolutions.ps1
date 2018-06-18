Param($WebAppURL);

Add-PSSnapin Microsoft.SharePoint.PowerShell 
set-executionpolicy remotesigned

function Header()
{
    Write-Host "-------------------------------------------"
    Write-Host -Fore Green "Start Uninstall Solutions"
	write-host -Fore Green "WebAppURL: " $WebAppURL 
	Write-Host "-------------------------------------------"
}
function Footer()
{
    Write-Host ""
    Write-Host "-------------------------------------------"
    Write-Host -Fore Green "End Uninstall Solutions"
    Write-Host "-------------------------------------------"
}

function Main($WspFolderPath, $WebAppURL)
{
	Write-Host $WspFolderPath
    $wspFiles = get-childitem $WspFolderPath | where {$_.Name -like "*.wsp"}
	ForEach($file in $wspFiles)
    {
		Write-Host " "
		Write-Host "File Name" $file.Name
        $solution = Get-SPSolution | Where{$_.Name -eq $file.Name}
        
		If ($solution -eq $null)
		{
 			Write-Host -Fore Yellow "The specified solution $file.Name was not found."
		}
		else
		{
    		Write-Host "Uninstall Solution... "$solution.Name -ForegroundColor Green
			if ($solution.ContainsWebApplicationResource)
			{
				Write-Host "Contains WebApplication Resource " -ForegroundColor DarkCyan
    			Uninstall-SPSolution -identity $solution.SolutionId -WebApplication $WebAppURL -confirm:$false 
			}
			else
			{
				Write-Host "Not contains WebApplication Resource " -ForegroundColor DarkCyan
				Uninstall-SPSolution -identity $solution.SolutionId -Confirm:$false 
			}

			while($solution.JobExists) 
			{     
				Write-Host  "Retraction in progress..."
				start-sleep -s 5 
			}
			Write-Host -Fore Green " -> retracted successfully!" 

			Write-Host -NoNewline "Deleting Solution..." 
			Remove-SPSolution $solution.SolutionId  -confirm:$false
			Write-Host -Fore Green " -> retracted successfully!"
		}
	}
}

Start-Transcript

Header

Main $(get-location).Path $WebAppURL

Footer

Stop-Transcript