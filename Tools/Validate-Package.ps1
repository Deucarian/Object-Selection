$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$requiredFiles = @(
    "package.json",
    "README.md",
    "CHANGELOG.md",
    "LICENSE.md",
    "CONTRIBUTING.md",
    "Runtime/Deucarian.ObjectSelection.asmdef",
    "Tests/EditMode/Deucarian.ObjectSelection.Tests.asmdef",
    "Samples~/PrimitiveSelection/Deucarian.ObjectSelection.Samples.PrimitiveSelection.asmdef",
    "Samples~/PrimitiveSelection/PrimitiveSelection.unity"
)

$requiredDirectories = @(
    "Runtime",
    "Tests/EditMode",
    "Samples~/PrimitiveSelection",
    "Tools",
    ".github/workflows"
)

foreach ($directory in $requiredDirectories) {
    $path = Join-Path $root $directory
    if (-not (Test-Path -LiteralPath $path -PathType Container)) {
        throw "Missing required directory: $directory"
    }
}

foreach ($file in $requiredFiles) {
    $path = Join-Path $root $file
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Missing required file: $file"
    }
}

$package = Get-Content -LiteralPath (Join-Path $root "package.json") -Raw | ConvertFrom-Json
if ($package.name -ne "com.deucarian.object-selection") {
    throw "Unexpected package name: $($package.name)"
}

if ($package.displayName -ne "Deucarian Object Selection") {
    throw "Unexpected package display name: $($package.displayName)"
}

if ($package.version -notmatch "^\d+\.\d+\.\d+$") {
    throw "Package version must be semver MAJOR.MINOR.PATCH: $($package.version)"
}

if ($package.dependencies."com.unity.modules.physics" -ne "1.0.0") {
    throw "Expected dependency com.unity.modules.physics version 1.0.0"
}

if ($package.dependencies."com.deucarian.logging" -ne "0.2.5") {
    throw "Expected dependency com.deucarian.logging version 0.2.5"
}

$runtimeAsmdef = Get-Content -LiteralPath (Join-Path $root "Runtime/Deucarian.ObjectSelection.asmdef") -Raw | ConvertFrom-Json
if ($runtimeAsmdef.name -ne "Deucarian.ObjectSelection") {
    throw "Unexpected runtime asmdef name: $($runtimeAsmdef.name)"
}

if ($runtimeAsmdef.references -notcontains "Deucarian.Logging") {
    throw "Runtime asmdef must reference Deucarian.Logging"
}

$testAsmdef = Get-Content -LiteralPath (Join-Path $root "Tests/EditMode/Deucarian.ObjectSelection.Tests.asmdef") -Raw | ConvertFrom-Json
if ($testAsmdef.references -notcontains "Deucarian.ObjectSelection") {
    throw "Tests asmdef must reference Deucarian.ObjectSelection"
}

$sampleAsmdef = Get-Content -LiteralPath (Join-Path $root "Samples~/PrimitiveSelection/Deucarian.ObjectSelection.Samples.PrimitiveSelection.asmdef") -Raw | ConvertFrom-Json
if ($sampleAsmdef.references -notcontains "Deucarian.ObjectSelection") {
    throw "Sample asmdef must reference Deucarian.ObjectSelection"
}

if ($sampleAsmdef.references -notcontains "Deucarian.Logging") {
    throw "Sample asmdef must reference Deucarian.Logging"
}

$forbiddenReferences = @(
    "CoreState",
    "Core.State",
    "UIBinding",
    "API",
    "Session",
    "UnityEngine.UI",
    "UnityEngine.EventSystems",
    "ServiceLocator"
)

$runtimeFiles = Get-ChildItem -LiteralPath (Join-Path $root "Runtime") -Recurse -File -Filter "*.cs"
foreach ($runtimeFile in $runtimeFiles) {
    $content = Get-Content -LiteralPath $runtimeFile.FullName -Raw
    foreach ($forbiddenReference in $forbiddenReferences) {
        if ($content -match [regex]::Escape($forbiddenReference)) {
            throw "Runtime file $($runtimeFile.Name) contains forbidden reference: $forbiddenReference"
        }
    }
}

$forbiddenProjectScaffolding = @("Assets", "Packages", "ProjectSettings")
foreach ($directory in $forbiddenProjectScaffolding) {
    $path = Join-Path $root $directory
    if (Test-Path -LiteralPath $path -PathType Container) {
        throw "Package repository should not contain Unity project scaffolding directory: $directory"
    }
}

$generatedArtifacts = Get-ChildItem -LiteralPath $root -Recurse -Force -File |
    Where-Object { $_.Name -match "\.(unitypackage|zip|tar|tgz)$" }
if ($generatedArtifacts.Count -gt 0) {
    throw "Generated artifacts are present in the package repository."
}

Write-Host "Deucarian Object Selection package validation passed."
