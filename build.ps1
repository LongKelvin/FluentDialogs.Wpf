<#
.SYNOPSIS
    Build script for FluentDialogs.Wpf NuGet package.

.PARAMETER Version
    The version number for the package (e.g., 1.0.0, 1.0.0-beta1).

.PARAMETER Configuration
    Build configuration. Defaults to Release.

.EXAMPLE
    .\build.ps1 -Version 1.0.0
    .\build.ps1 -Version 1.0.0-beta1 -Configuration Debug
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $false)]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectPath = Join-Path $ScriptDir "FluentDialogs.Wpf.csproj"
$ArtifactsDir = Join-Path $ScriptDir "artifacts"

Write-Host "Building FluentDialogs.Wpf v$Version" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path $ProjectPath)) {
    Write-Error "Project file not found: $ProjectPath"
    exit 1
}

if (Test-Path $ArtifactsDir) {
    Write-Host "Cleaning artifacts directory..." -ForegroundColor Yellow
    Remove-Item -Path $ArtifactsDir -Recurse -Force
}

New-Item -ItemType Directory -Path $ArtifactsDir -Force | Out-Null

Write-Host "Restoring dependencies..." -ForegroundColor Yellow
dotnet restore $ProjectPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed"
    exit $LASTEXITCODE
}

Write-Host "Building project..." -ForegroundColor Yellow
dotnet build $ProjectPath -c $Configuration --no-restore /p:Version=$Version
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit $LASTEXITCODE
}

Write-Host "Creating NuGet package..." -ForegroundColor Yellow
dotnet pack $ProjectPath -c $Configuration --no-build -o $ArtifactsDir /p:Version=$Version
if ($LASTEXITCODE -ne 0) {
    Write-Error "Pack failed"
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "Artifacts:" -ForegroundColor Cyan
Get-ChildItem $ArtifactsDir | ForEach-Object { Write-Host "  $_" -ForegroundColor White }
