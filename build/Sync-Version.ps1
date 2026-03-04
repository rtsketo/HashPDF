param(
    [switch]$Check
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$versionPath = Join-Path $repoRoot "VERSION"
$assemblyInfoPath = Join-Path $repoRoot "src\HashPDF.WinForms\Properties\AssemblyInfo.cs"
$installerPath = Join-Path $repoRoot "installer\HashPDF.iss"

if (-not (Test-Path $versionPath)) {
    throw "VERSION file was not found."
}

$version = (Get-Content $versionPath -Raw).Trim()
if (-not ($version -match '^\d+\.\d+\.\d+$')) {
    throw "VERSION must follow semantic version format X.Y.Z."
}

$assemblyVersion = "$version.0"

function Update-Content {
    param(
        [string]$Path,
        [hashtable]$Replacements
    )

    $original = Get-Content $Path -Raw
    $updated = $original

    foreach ($pattern in $Replacements.Keys) {
        $updated = [regex]::Replace($updated, $pattern, $Replacements[$pattern])
    }

    if ($Check) {
        if ($updated -ne $original) {
            throw "Version metadata is out of sync in $Path. Run build/Sync-Version.ps1 and commit the result."
        }

        return
    }

    if ($updated -ne $original) {
        Set-Content -Path $Path -Value $updated -NoNewline
    }
}

Update-Content -Path $assemblyInfoPath -Replacements @{
    '\[assembly: AssemblyVersion\(".*?"\)\]' = "[assembly: AssemblyVersion(`"$assemblyVersion`")]"
    '\[assembly: AssemblyFileVersion\(".*?"\)\]' = "[assembly: AssemblyFileVersion(`"$assemblyVersion`")]"
}

Update-Content -Path $installerPath -Replacements @{
    '#define MyAppVersion ".*?"' = "#define MyAppVersion `"$version`""
}

Write-Host "Version metadata synchronized for $version"
