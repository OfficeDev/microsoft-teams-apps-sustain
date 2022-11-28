# $tenantId = '<<GUID - Id of tenant>>'
# $subscriptionId = '<<GUID - Id of subscription>>'
# $resource_group_name = '<<Name of resource group eg. sustain-app-demo>>'
# $app_service_web_name = '<<Name of Azure App Service of Web eg. sustain-app-web>>'
# $app_service_api_name = '<<Name of Azure App Service for API eg. sustain-app-api>>'
# $project_path = '<<Local path of project eg. C:\Users\<UserName>\Desktop\SUSTAINABILITY SUPPORT\microsoft-teams-apps-sustain-main_updated\microsoft-teams-apps-sustain-main\>>'

# Parameters
param(
    [string] [parameter(mandatory=$true)] $tenantid,
    [string] [parameter(mandatory=$true)] $subscriptionid,
    [string] [parameter(mandatory=$true)] $resource_group_name,
    [string] [parameter(mandatory=$true)] $app_service_api_name,
    [string] [parameter(mandatory=$true)] $app_service_web_name,
    [string] [parameter(mandatory=$true)] $project_path
)

$dateTimeNow = Get-Date
$outputPrefix = '[Deployment - ' + $dateTimeNow + ']'
$WarningPreference = "SilentlyContinue"

Write-Output "$outputPrefix - Deployment Started."

Write-Output "$outputPrefix - Connecting to Azure."

Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process

# Connect to specific tenant and subscription
Import-Module Az.Accounts >$null 2>&1
Connect-AzAccount -TenantId $tenantId >$null 2>&1
Set-AzContext -TenantId $tenantId -Subscription $subscriptionId >$null 2>&1

# BUILD API

Write-Output "$outputPrefix - Installing dotnet sdk."
# Install DOTNET SDK
cd $project_path
& .\Deployment\Azure\dotnet-install.ps1

Write-Output "$outputPrefix - Declaring dotnet invoker function."

# Function for invoking DOTNET Commands
function Invoke-Dotnet {
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true)]
        [System.String]
        $Command,

        [Parameter(Mandatory = $true)]
        [System.String]
        $Arguments
    )

    $DotnetArgs = @()
    $DotnetArgs = $DotnetArgs + $Command
    $DotnetArgs = $DotnetArgs + ($Arguments -split "\s+")

    [void]($Output = & dotnet $DotnetArgs)

    # Should throw if the last command failed.
    if ($LASTEXITCODE -ne 0) {
        Write-Warning -Message ($Output -join "; ")
        throw "There was an issue running the specified dotnet command."
    }
}

Write-Output "$outputPrefix - Changing directory to project path." 
# Change directory to project path
cd $project_path

Write-Output "$outputPrefix - Build & Publish API Project."
# Build & Publish API Project
try {
    Invoke-Dotnet -Command build -Arguments ".\Microsoft.Teams.Apps.Sustainability.sln --configuration Release"
    Invoke-Dotnet -Command  publish -Arguments ".\WebAPI\WebAPI.csproj -c Release"
} catch { }

Write-Output "$outputPrefix - Zipping API build files."

$var_api_to_zip_path = Join-Path $project_path 'WebAPI\bin\Release\net6.0\publish\*'
$var_api_zipped_path = Join-Path $project_path 'WebAPI\bin\Release\net6.0\publish\app.zip'

# Zip all files from bin
Compress-Archive -Path $var_api_to_zip_path -DestinationPath $var_api_zipped_path -Force

# BUILD WEB 

Write-Output "$outputPrefix - Zipping WEB build files."

$var_web_to_zip_path = Join-Path $project_path 'WebUI\build\*'
$var_web_zipped_path = Join-Path $project_path 'WebUI\build\app.zip'

# zip all files from build
Compress-Archive -Path $var_web_to_zip_path -DestinationPath $var_web_zipped_path -Force


# Import Azure Websites Module
try {
    Import-Module Az.Websites >$null 2>&1

    Write-Output "$outputPrefix - Deploying API build files."
    # DEPLOY API
    $result_app_service_api = Get-AzWebApp -ResourceGroupName $resource_group_name -Name $app_service_api_name
    Publish-AzWebApp -WebApp $result_app_service_api -ArchivePath $var_api_zipped_path >$null 2>&1

    Write-Output "$outputPrefix - Deploying WEB build files."
    # DEPLOY WEB
    $result_app_service_web = Get-AzWebApp -ResourceGroupName $resource_group_name -Name $app_service_web_name
    Publish-AzWebApp -WebApp $result_app_service_web -ArchivePath $var_web_zipped_path >$null 2>&1

    Write-Output "$outputPrefix - Deployment finished successfully."
} catch { }
