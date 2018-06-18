Param($SiteURL,$Port);

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{
    Add-PSSnapin "Microsoft.SharePoint.PowerShell"
}
if($SiteURL -eq $null)
{
	$SiteURL = "http://localhost"
}
$locationPath = ($(get-location).Path).Trim()
function Main-DeployRequestDiplomaModule($Port)
{
	if($Port -eq $null)
	{
		$Port = 80
	}
	#NOTE: 
		# Deploy for DEV: Pusblish 'RBVH.Stada.Intranet.Resources','RBVH.Stada.Intranet.SiteColumns','RBVH.Stada.Intranet.ContentTypes',
		#						'RBVH.Stada.Intranet.ListDefinitions','RBVH.Stada.Intranet.WebPages','RBVH.Stada.Intranet.SiteTemplate'
		#						'RBVH.Stada.Intranet.Branding','RBVH.Stada.Intranet.ListEventReceiver','RBVH.Stada.Intranet.Webservices'
		#						into 'Stada.Intranet.SP2016\PowerShellScript\Solutions' folder (*1)

		# Deploy for Testing (sptest) & STADA server: Pusblish only 'RBVH.Stada.Intranet.Main' into 'Stada.Intranet.SP2016\PowerShellScript\Solutions' folder (*2)
		
	
	# Step 1: Update Solution (Resources, List Definition, WebPages, ....
	DeploySolutions

	# Step 2: Copy [RBVHStadaLists.resx] as [RBVHStadaLists.en-US.resx] and [RBVHStadaWebpages.resx] as [RBVHStadaWebpages.en-US.resx] at [C:\inetpub\wwwroot\wss\VirtualDirectories\[Port]\App_GlobalResources\] and [C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\16\Resources\]
	CopyResource

	# Step 3: Reset IIS
	iisreset

	# Step 4: Deactive List Definition-Active List Definition
	Deactivate-ActivateFeature

	# Step 5: Import Data
		# Step 5.1: Import [WorkflowSteps]
		# Step 5.2: Import [WorkflowEmailTemplates]
		# Step 5.3: Import [PermissionGroup]. Trưởng Phòng, Ban Giám Đốc. Trưởng Phòng - Hành Chánh.
		# Step 5.4: Create folder [RequestForDiplomaSupplies] for [SupportingDocuments] 
	ImportData

	# Step 6: Grant Permission
		# Step 6.1: Grant read permission for all of Groups: [RequestForDiplomaSupplies], [RequestDiplomaDetails]
		# Step 6.2: Grant Contribute permission for all of Groups: [RequestForDiplomaSupplies], [RequestDiplomaDetails]
	GrantPermission

	# Step7: Update formula for 'Status Order' colunm
	UpdateFormula
}

function DeploySolutions()
{
	Invoke-Expression ($locationPath + "\InstallSolutions.ps1 -SiteURL " + $SiteURL)
	#Invoke-Expression ($locationPath + "\InstallSolutions.ps1 -SiteURL " + $SiteURL)
}

function CopyResource()
{
	Write-Host "Copy resource ... "
	Copy-Item ("C:\inetpub\wwwroot\wss\VirtualDirectories\" + $Port + "\App_GlobalResources\RBVHStadaLists.resx") ("C:\inetpub\wwwroot\wss\VirtualDirectories\" + $Port + "\App_GlobalResources\RBVHStadaLists.en-US.resx") 
	Copy-Item ("C:\inetpub\wwwroot\wss\VirtualDirectories\" + $Port + "\App_GlobalResources\RBVHStadaWebpages.resx") ("C:\inetpub\wwwroot\wss\VirtualDirectories\" + $Port + "\App_GlobalResources\RBVHStadaWebpages.en-US.resx") 

	Copy-Item "C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\16\Resources\RBVHStadaLists.resx" "C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\16\Resources\RBVHStadaLists.en-US.resx"
	Copy-Item "C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\16\Resources\RBVHStadaWebpages.resx" "C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\16\Resources\RBVHStadaWebpages.en-US.resx"
	Write-Host "Done Coping resource"
}

function Deactivate-ActivateFeature()
{
	Write-Host ($locationPath + "\ActiveFeatures.ps1 -SiteURL " + $SiteURL)
	Invoke-Expression  ($locationPath + "\ActiveFeatures.ps1 -SiteURL " + $SiteURL)
}

function ImportData
{
	Invoke-Expression ($locationPath + "\ImportWorkflowStepList.ps1 -SiteURL " + $SiteURL)
	Invoke-Expression ($locationPath + "\ImportWorkflowEmailTemplate.ps1 -SiteURL " + $SiteURL)
	Invoke-Expression ($locationPath + "\ImportPermissionPage.ps1 -SiteURL " + $SiteURL)
	Invoke-Expression ($locationPath + "\CreateFolderInFormList.ps1 -SiteURL " + $SiteURL)
}

function GrantPermission
{
	Invoke-Expression ($locationPath + "\AssignReadPermissionForAllLists.ps1 -SiteURL " + $SiteURL)
	Invoke-Expression ($locationPath + "\AssignContributorPermissionByModule.ps1 -SiteURL " + $SiteURL)
}
function UpdateFormula()
{
	Invoke-Expression ($locationPath + "\SetStatusOrderFormula.ps1 -SiteURL " + $SiteURL + " -ListName 'Request for diploma supplies'")
}

Start-Transcript
Write-Host "STARTING..." -ForegroundColor Blue
Main-DeployRequestDiplomaModule $Port
Write-Host "DONE" -ForegroundColor Blue
Stop-Transcript