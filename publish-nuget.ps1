<#
.SYNOPSIS
    Publish FluentDialogs.Wpf NuGet packages to nuget.org.

.PARAMETER ApiKey
    NuGet API key for publishing.

.PARAMETER Source
    NuGet source URL. Defaults to nuget.org.

.PARAMETER SkipDuplicate
    Skip packages that already exist on the server. Defaults to true.

.EXAMPLE
    .\publish-nuget.ps1 -ApiKey "your-api-key"
    .\publish-nuget.ps1 -ApiKey $env:NUGET_API_KEY
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ApiKey,

    [Parameter(Mandatory = $false)]
    [string]$Source = "https://api.nuget.org/v3/index.json",

    [Parameter(Mandatory = $false)]
    [switch]$SkipDuplicate = $true
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ArtifactsDir = Join-Path $ScriptDir "artifacts"

Write-Host "Publishing NuGet packages from $ArtifactsDir" -ForegroundColor Cyan
Write-Host "Source: $Source" -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path $ArtifactsDir)) {
    Write-Error "Artifacts directory not found: $ArtifactsDir. Run build.ps1 first."
    exit 1
}

$packages = Get-ChildItem -Path $ArtifactsDir -Filter "*.nupkg" -Exclude "*.symbols.nupkg"

if ($packages.Count -eq 0) {
    Write-Error "No .nupkg files found in $ArtifactsDir"
    exit 1
}

$failed = $false

foreach ($package in $packages) {
    Write-Host "Pushing $($package.Name)..." -ForegroundColor Yellow
    
    $args = @("nuget", "push", $package.FullName, "--api-key", $ApiKey, "--source", $Source)
    
    if ($SkipDuplicate) {
        $args += "--skip-duplicate"
    }
    
    & dotnet @args
    
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Failed to push $($package.Name)"
        $failed = $true
    }
    else {
        Write-Host "Successfully pushed $($package.Name)" -ForegroundColor Green
    }
}

if ($failed) {
    Write-Error "One or more packages failed to publish"
    exit 1
}

Write-Host ""
Write-Host "All packages published successfully!" -ForegroundColor Green
