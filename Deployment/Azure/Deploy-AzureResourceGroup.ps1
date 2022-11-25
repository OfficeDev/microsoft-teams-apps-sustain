#Requires -Version 3.0

 function Format-ValidationOutput {
        param ($ValidationOutput, [int] $Depth = 0)
        Set-StrictMode -Off
        return @($ValidationOutput | Where-Object { $_ -ne $null } | ForEach-Object { @('  ' * $Depth + ': ' + $_.Message) + @(Format-ValidationOutput @($_.Details) ($Depth + 1)) })
    }

function DeployAzureResources{
    Param(
        [string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
        [string] [Parameter(Mandatory=$true)] $ResourceGroupName,
        [switch] $UploadArtifacts,
        [string] $StorageAccountName,
        [string] $StorageContainerName = $ResourceGroupName.ToLowerInvariant() + '-stageartifacts',
        [string] $TemplateFile = 'azuredeploy.json',
        [string] $TemplateParametersFile = 'azuredeploy.parameters.json',
        [string] $ArtifactStagingDirectory = '.',
        [string] $DSCSourceFolder = 'DSC',
        [switch] $ValidateOnly
    )

    try {
        [Microsoft.Azure.Common.Authentication.AzureSession]::ClientFactory.AddUserAgent("VSAzureTools-$UI$($host.name)".replace(' ','_'), '3.0.0')
    } catch { }

    $ErrorActionPreference = 'Stop'
    Set-StrictMode -Version 3

   

    $OptionalParameters = New-Object -TypeName Hashtable
    $TemplateFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $TemplateFile))
    $TemplateParametersFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $TemplateParametersFile))

    if ($UploadArtifacts) {
        # Convert relative paths to absolute paths if needed
        $ArtifactStagingDirectory = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $ArtifactStagingDirectory))
        $DSCSourceFolder = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $DSCSourceFolder))

        # Parse the parameter file and update the values of artifacts location and artifacts location SAS token if they are present
        $JsonParameters = Get-Content $TemplateParametersFile -Raw | ConvertFrom-Json
        if (($JsonParameters | Get-Member -Type NoteProperty 'parameters') -ne $null) {
            $JsonParameters = $JsonParameters.parameters
        }
        $ArtifactsLocationName = '_artifactsLocation'
        $ArtifactsLocationSasTokenName = '_artifactsLocationSasToken'
        $OptionalParameters[$ArtifactsLocationName] = $JsonParameters | Select -Expand $ArtifactsLocationName -ErrorAction Ignore | Select -Expand 'value' -ErrorAction Ignore
        $OptionalParameters[$ArtifactsLocationSasTokenName] = $JsonParameters | Select -Expand $ArtifactsLocationSasTokenName -ErrorAction Ignore | Select -Expand 'value' -ErrorAction Ignore

        # Create DSC configuration archive
        if (Test-Path $DSCSourceFolder) {
            $DSCSourceFilePaths = @(Get-ChildItem $DSCSourceFolder -File -Filter '*.ps1' | ForEach-Object -Process {$_.FullName})
            foreach ($DSCSourceFilePath in $DSCSourceFilePaths) {
                $DSCArchiveFilePath = $DSCSourceFilePath.Substring(0, $DSCSourceFilePath.Length - 4) + '.zip'
                Publish-AzureRmVMDscConfiguration $DSCSourceFilePath -OutputArchivePath $DSCArchiveFilePath -Force -Verbose
            }
        }

        # Create a storage account name if none was provided
        if ($StorageAccountName -eq '') {
            $StorageAccountName = 'stage' + ((Get-AzureRmContext).Subscription.SubscriptionId).Replace('-', '').substring(0, 19)
        }

        $StorageAccount = (Get-AzureRmStorageAccount | Where-Object{$_.StorageAccountName -eq $StorageAccountName})

        # Create the storage account if it doesn't already exist
        if ($StorageAccount -eq $null) {
            $StorageResourceGroupName = 'ARM_Deploy_Staging'
            New-AzureRmResourceGroup -Location "$ResourceGroupLocation" -Name $StorageResourceGroupName -Force
            $StorageAccount = New-AzureRmStorageAccount -StorageAccountName $StorageAccountName -Type 'Standard_LRS' -ResourceGroupName $StorageResourceGroupName -Location "$ResourceGroupLocation"
        }

        # Generate the value for artifacts location if it is not provided in the parameter file
        if ($OptionalParameters[$ArtifactsLocationName] -eq $null) {
            $OptionalParameters[$ArtifactsLocationName] = $StorageAccount.Context.BlobEndPoint + $StorageContainerName + '/'
        }

        # Copy files from the local storage staging location to the storage account container
        New-AzureStorageContainer -Name $StorageContainerName -Context $StorageAccount.Context -ErrorAction SilentlyContinue *>&1

        $ArtifactFilePaths = Get-ChildItem $ArtifactStagingDirectory -Recurse -File | ForEach-Object -Process {$_.FullName}
        foreach ($SourcePath in $ArtifactFilePaths) {
            Set-AzureStorageBlobContent -File $SourcePath -Blob $SourcePath.Substring($ArtifactStagingDirectory.length + 1) `
                -Container $StorageContainerName -Context $StorageAccount.Context -Force
        }

        # Generate a 4 hour SAS token for the artifacts location if one was not provided in the parameters file
        if ($OptionalParameters[$ArtifactsLocationSasTokenName] -eq $null) {
            $OptionalParameters[$ArtifactsLocationSasTokenName] = ConvertTo-SecureString -AsPlainText -Force `
                (New-AzureStorageContainerSASToken -Container $StorageContainerName -Context $StorageAccount.Context -Permission r -ExpiryTime (Get-Date).AddHours(4))
        }
    }

    # Create the resource group only when it doesn't already exist
    if ((Get-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Verbose -ErrorAction SilentlyContinue) -eq $null) {
        New-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Verbose -Force -ErrorAction Stop
    }

    if ($ValidateOnly) {
        $ErrorMessages = Format-ValidationOutput (Test-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName `
                                                                                    -TemplateFile $TemplateFile `
                                                                                    -TemplateParameterFile $TemplateParametersFile `
                                                                                    @OptionalParameters)
        if ($ErrorMessages) {
            Write-Output '', 'Validation returned the following errors:', @($ErrorMessages), '', 'Template is invalid.'
        }
        else {
            Write-Output '', 'Template is valid.'
        }
    }
    else {
        New-AzureRmResourceGroupDeployment -Name ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
                                        -ResourceGroupName $ResourceGroupName `
                                        -TemplateFile $TemplateFile `
                                        -TemplateParameterFile $TemplateParametersFile `
                                        @OptionalParameters `
                                        -Force -Verbose `
                                        -ErrorVariable ErrorMessages
        if ($ErrorMessages) {
            Write-Output '', 'Template deployment returned the following errors:', @(@($ErrorMessages) | ForEach-Object { $_.Exception.Message.TrimEnd("`r`n") })
        }
    }
}

function CreateAzureADApp {
    param(
        [Parameter(Mandatory = $true)] [string] $AppName,
		[Parameter(Mandatory = $false)] [bool] $ResetAppSecret = $true,
        [Parameter(Mandatory = $false)] [bool] $MultiTenant = $true,
        [Parameter(Mandatory = $false)] [bool] $AllowImplicitFlow
    )
        
    try {
        Write-Host -message "`r`nCreating Azure AD App: $appName..."

        # Check if the app already exists - script has been previously executed
        $app = Get-AzureADApplication -Filter "DisplayName eq '$appName'"

        if (-not ([string]::IsNullOrEmpty($app))) {

            # Update Azure AD app registration using CLI
            $confirmationTitle = "The Azure AD app '$appName' already exists. If you proceed, this will update the existing app configuration."
            $confirmationQuestion = "Do you want to proceed?"
            $confirmationChoices = "&Yes", "&No" # 0 = Yes, 1 = No
            
            $updateDecision = $Host.UI.PromptForChoice($confirmationTitle, $confirmationQuestion, $confirmationChoices, 1)
            if ($updateDecision -eq 0) {
                WriteI -message "Updating the existing app..."

                az ad app update --id $app.appId #--available-to-other-tenants $MultiTenant --oauth2-allow-implicit-flow $AllowImplicitFlow

                WriteI -message "Waiting for app update to finish..."

                Start-Sleep -s 10

                WriteS -message "Azure AD App: $appName is updated."
            } else {
                WriteE -message "Deployment canceled. Please use a different name for the Azure AD app and try again."
                return $null
            }
        } else {
            # Create Azure AD app registration using CLI
             az ad app create --display-name $appName #--available-to-other-tenants $MultiTenant --oauth2-allow-implicit-flow $AllowImplicitFlow

            Write-Host "Waiting for app creation to finish..."

            Start-Sleep -s 10

            Write-Host "Azure AD App: $appName is created."
        }

        Write-Host "Azure AD App: $appName registered successfully."
        return ""
    }
    catch {
        $errorMessage = $_.Exception.Message
        Write-Host "Failed to register/configure the Azure AD app. Error message: $errorMessage"
    }
    return $null
}

function CheckAllPrerequisites{
    # Check if Azure CLI is installed.
    Write-Host "Checking if Azure CLI is installed."
    $localPath = [Environment]::GetEnvironmentVariable("ProgramFiles(x86)")
    if ($localPath -eq $null) {
        $localPath = "C:\Program Files (x86)"
    }

    $localPath = $localPath + "\Microsoft SDKs\Azure\CLI2"
    If (-not(Test-Path -Path $localPath)) {
        Write-Host "Azure CLI is not installed!"
        $confirmationtitle      = "Please select YES to install Azure CLI."
        $confirmationquestion   = "Do you want to proceed?"
        $confirmationchoices    = "&yes", "&no" # 0 = yes, 1 = no
            
        $updatedecision = $host.ui.promptforchoice($confirmationtitle, $confirmationquestion, $confirmationchoices, 1)
        if ($updatedecision -eq 0) {
            Write-Host "Installing Azure CLI ..."
            Invoke-WebRequest -Uri https://azcliprod.blob.core.windows.net/msi/azure-cli-2.30.0.msi -OutFile .\AzureCLI.msi; Start-Process msiexec.exe -Wait -ArgumentList '/I AzureCLI.msi /quiet'; rm .\AzureCLI.msi
            Write-Host "Azure CLI is installed! Please close this PowerShell window and re-run this script in a new PowerShell session."
            EXIT
        } else {
            Write-Host "Azure CLI is not installed.`nPlease install the CLI from https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest and re-run this script in a new PowerShell session"
            EXIT
        }
    } else {
        Write-Host "Azure CLI is installed."
    }

    # Installing required modules
    Write-Host "Checking if the required modules are installed..."
    $isAvailable = $true
    if ((Get-Module -ListAvailable -Name "Az.*")) {
        Write-Host "Az module is available."
    } else {
        Write-Host "Az module is missing."
        $isAvailable = $false
    }

    if ((Get-Module -ListAvailable -Name "AzureAD")) {
        Write-Host "AzureAD module is available."
    } else {
        Write-Host "AzureAD module is missing."
        $isAvailable = $false
    }

    if ((Get-Module -ListAvailable -Name "WriteAscii")) {
        Write-Host "WriteAscii module is available."
    } else {
        Write-Host "WriteAscii module is missing."
        $isAvailable = $false
    }

    if (-not $isAvailable)
    {
        $confirmationTitle = Write-Host "The script requires the following modules to deploy: `n 1.Az module`n 2.AzureAD module `n 3.WriteAscii module`nIf you proceed, the script will install the missing modules."
        $confirmationQuestion = "Do you want to proceed?"
        $confirmationChoices = "&Yes", "&No" # 0 = Yes, 1 = No
                
        $updateDecision = $Host.UI.PromptForChoice($confirmationTitle, $confirmationQuestion, $confirmationChoices, 1)
            if ($updateDecision -eq 0) {
                if (-not (Get-Module -ListAvailable -Name "Az.*")) {
                    Write-Host "Installing AZ module..."
                    Install-Module Az -AllowClobber -Scope CurrentUser
                }

                if (-not (Get-Module -ListAvailable -Name "AzureAD")) {
                    Write-Host "Installing AzureAD module..."
                    Install-Module AzureAD -Scope CurrentUser -Force
                }
                
                if (-not (Get-Module -ListAvailable -Name "WriteAscii")) {
                    Write-Host "Installing WriteAscii module..."
                    Install-Module WriteAscii -Scope CurrentUser -Force
                }
            } else {
                Write-Host "You may install the modules manually by following the below link. Please re-run the script after the modules are installed. `nhttps://docs.microsoft.com/en-us/powershell/module/powershellget/install-module?view=powershell-7"
                EXIT
            }
    } else {
        Write-Host "All the modules are available!"
    }
}

CheckAllPrerequisites

DeployAzureResources 

$parametersListContent = Get-Content '.\azuredeploy.parameters.json' -ErrorAction Stop
$parameters = $parametersListContent | ConvertFrom-Json

#Initialize connections - Azure Az/CLI/Azure AD
Write-Host "Login with with your Azure subscription account. Launching Azure sign-in window..."
Connect-AzAccount -Subscription $parameters.parameters.subscription_id.value -Tenant $parameters.parameters.tenant_id.value -ErrorAction Stop
$user = az login --tenant $parameters.parameters.tenant_id.value
if ($LASTEXITCODE -ne 0) {
    WriteE -message "Login failed for user..."
    EXIT
}

Write-Host "Azure AD sign-in..."
$ADaccount = Connect-AzureAD -Tenant $parameters.parameters.tenant_id.value -ErrorAction Stop
$userAlias = (($user | ConvertFrom-Json) | where {$_.id -eq $parameters.parameters.subscription_id.value}).user.name
# Create or Update User App

$usersApp = $parameters.parameters.app_name.value
$userAppCred = $null

$userAppCred = CreateAzureADApp $usersApp


