[CmdletBinding(DefaultParameterSetName = 'RegularBuild')]
param (
    [ValidateSet("debug", "release")]
    [string]$Configuration = 'debug',
    [int]$BuildNumber,
    [switch]$SkipRestore,
    [switch]$SkipSubModules,
    [switch]$CleanCache,
    [string]$SimpleVersion = '1.0.0',
    [string]$SemanticVersion = '1.0.0-zlocal',
    [string]$PackageSuffix,
    [string]$Branch,
    [string]$CommitSHA,
    [string]$BuildBranch = '5295c6e0d2ae7357fccf01e48c56b768b192f022' # DevSkim: ignore DS173237. It's a commit hash.
)

Set-StrictMode -Version 1.0

# This script should fail the build if any issue occurs.
trap {
    Write-Host "BUILD FAILED: $_" -ForegroundColor Red
    Write-Host "ERROR DETAILS:" -ForegroundColor Red
    Write-Host $_.Exception -ForegroundColor Red
    Write-Host ("`r`n" * 3)
    exit 1
}

if (-not (Test-Path "$PSScriptRoot/build")) {
    New-Item -Path "$PSScriptRoot/build" -ItemType "directory"
}

Invoke-WebRequest -UseBasicParsing -Uri "https://raw.githubusercontent.com/NuGet/ServerCommon/$BuildBranch/build/init.ps1" -OutFile "$PSScriptRoot/build/init.ps1"
. "$PSScriptRoot/build/init.ps1" -BuildBranch "$BuildBranch"

Function Clean-Tests {
    [CmdletBinding()]
    param()
    
    Trace-Log 'Cleaning test results'
    
    Remove-Item (Join-Path $PSScriptRoot "Results.*.xml")
}
    
Write-Host ("`r`n" * 3)
Trace-Log ('=' * 60)

$startTime = [DateTime]::UtcNow
if (-not $BuildNumber) {
    $BuildNumber = Get-BuildNumber
}
Trace-Log "Build #$BuildNumber started at $startTime"

$BuildErrors = @()

Invoke-BuildStep 'Reset all submodules' { Reset-Submodules } `
    -skip:($SkipSubModules) `
    -ev +BuildErrors

Invoke-BuildStep "Update NuGetGallery submodule" {
    Invoke-Git 'submodule', 'update', '--init', '--', 'src/NuGet.Status/NuGetGallery'
} `
    -skip:($SkipSubModules) `
    -ev +BuildErrors
    
Invoke-BuildStep 'Getting private build tools' { Install-PrivateBuildTools } `
    -ev +BuildErrors

Invoke-BuildStep 'Cleaning test results' { Clean-Tests } `
    -ev +BuildErrors

Invoke-BuildStep 'Installing NuGet.exe' { Install-NuGet } `
    -ev +BuildErrors
    
Invoke-BuildStep 'Clearing package cache' { Clear-PackageCache } `
    -skip:(-not $CleanCache) `
    -ev +BuildErrors
    
Invoke-BuildStep 'Clearing artifacts' { Clear-Artifacts } `
    -ev +BuildErrors

Invoke-BuildStep 'Restoring solution packages' { `
        Install-SolutionPackages -path (Join-Path $PSScriptRoot ".nuget\packages.config") -output (Join-Path $PSScriptRoot "packages") -excludeversion } `
    -skip:$SkipRestore `
    -ev +BuildErrors
    
Invoke-BuildStep 'Set version metadata in AssemblyInfo.cs' {
    $Paths = `
    (Join-Path $PSScriptRoot "src\NuGet.Status\Properties\AssemblyInfo.g.cs")

    Foreach ($Path in $Paths) {
        Set-VersionInfo -Path $Path -Version $SimpleVersion -Branch $Branch -Commit $CommitSHA
    }
} `
    -ev +BuildErrors

Invoke-BuildStep 'Building solution' { 
    $SolutionPath = Join-Path $PSScriptRoot "src\NuGet.Status.sln"
    Build-Solution $Configuration $BuildNumber -MSBuildVersion "15" $SolutionPath -SkipRestore:$SkipRestore `
} `
    -ev +BuildErrors

Trace-Log ('-' * 60)

## Calculating Build time
$endTime = [DateTime]::UtcNow
Trace-Log "Build #$BuildNumber ended at $endTime"
Trace-Log "Time elapsed $(Format-ElapsedTime ($endTime - $startTime))"

Trace-Log ('=' * 60)

if ($BuildErrors) {
    $ErrorLines = $BuildErrors | % { ">>> $($_.Exception.Message)" }
    Error-Log "Builds completed with $($BuildErrors.Count) error(s):`r`n$($ErrorLines -join "`r`n")" -Fatal
}

Write-Host ("`r`n" * 3)