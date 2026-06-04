param(
    [string]$Version = "9.9.9",
    [int]$Port = 8765,
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$appOutput = Join-Path $repoRoot "src\HashPDF.WinForms\bin\$Configuration"
$appExe = Join-Path $appOutput "HashPDF.exe"
$updaterExe = Join-Path $appOutput "HashPDF.Updater.exe"
$currentVersion = (Get-Content (Join-Path $repoRoot "VERSION") -Raw).Trim()

if (-not (Test-Path $appExe) -or -not (Test-Path $updaterExe)) {
    throw "Build $Configuration first so HashPDF.exe and HashPDF.Updater.exe exist in $appOutput."
}

$demoRoot = Join-Path $env:TEMP "HashPDF-update-demo"
$feedRoot = Join-Path $demoRoot "feed"
$installRoot = Join-Path $demoRoot "install"

if (Test-Path $demoRoot) {
    Remove-Item -LiteralPath $demoRoot -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $feedRoot | Out-Null
New-Item -ItemType Directory -Force -Path $installRoot | Out-Null

Copy-Item $appExe (Join-Path $installRoot "HashPDF.exe") -Force
Copy-Item $updaterExe (Join-Path $installRoot "HashPDF.Updater.exe") -Force
Copy-Item $appExe (Join-Path $feedRoot "HashPDF.exe") -Force
Copy-Item $updaterExe (Join-Path $feedRoot "HashPDF.Updater.exe") -Force

$baseUrl = "http://127.0.0.1:$Port"
$tagName = "v$Version"
$appAsset = Join-Path $feedRoot "HashPDF.exe"
$updaterAsset = Join-Path $feedRoot "HashPDF.Updater.exe"
$manifestPath = Join-Path $feedRoot "HashPDF.update.json"
$latestPath = Join-Path $feedRoot "latest.json"
$releasePath = Join-Path $feedRoot "release-$tagName.json"

$manifest = [ordered]@{
    version = $Version
    files = @(
        [ordered]@{
            path = "HashPDF.exe"
            assetName = "HashPDF.exe"
            sha256 = (Get-FileHash -Algorithm SHA256 $appAsset).Hash.ToLowerInvariant()
            size = (Get-Item $appAsset).Length
        },
        [ordered]@{
            path = "HashPDF.Updater.exe"
            assetName = "HashPDF.Updater.exe"
            sha256 = (Get-FileHash -Algorithm SHA256 $updaterAsset).Hash.ToLowerInvariant()
            size = (Get-Item $updaterAsset).Length
        }
    )
}
$manifest | ConvertTo-Json -Depth 4 | Set-Content -Path $manifestPath -Encoding UTF8

function New-ReleaseJson {
    param(
        [string]$Tag,
        [string]$SemanticVersion
    )

    [ordered]@{
        tag_name = $Tag
        name = "HashPDF $Tag Local Update Test"
        html_url = "$baseUrl/"
        draft = $false
        prerelease = $false
        assets = @(
            [ordered]@{
                name = "HashPDF.update.json"
                browser_download_url = "$baseUrl/HashPDF.update.json"
                size = (Get-Item $manifestPath).Length
            },
            [ordered]@{
                name = "HashPDF.exe"
                browser_download_url = "$baseUrl/HashPDF.exe"
                size = (Get-Item $appAsset).Length
            },
            [ordered]@{
                name = "HashPDF.Updater.exe"
                browser_download_url = "$baseUrl/HashPDF.Updater.exe"
                size = (Get-Item $updaterAsset).Length
            }
        )
    }
}

New-ReleaseJson -Tag $tagName -SemanticVersion $Version |
    ConvertTo-Json -Depth 5 |
    Set-Content -Path $latestPath -Encoding UTF8
Copy-Item $latestPath $releasePath -Force

$prefix = "$baseUrl/"
$serverJob = Start-Job -ArgumentList $feedRoot, $prefix -ScriptBlock {
    param($FeedRoot, $Prefix)

    $listener = New-Object System.Net.HttpListener
    $listener.Prefixes.Add($Prefix)
    $listener.Start()

    try {
        while ($listener.IsListening) {
            $context = $listener.GetContext()
            $relativePath = [Uri]::UnescapeDataString($context.Request.Url.AbsolutePath.TrimStart("/"))
            if ([string]::IsNullOrEmpty($relativePath)) {
                $relativePath = "latest.json"
            }

            $filePath = Join-Path $FeedRoot $relativePath
            if (-not (Test-Path -LiteralPath $filePath)) {
                $context.Response.StatusCode = 404
                $bytes = [Text.Encoding]::UTF8.GetBytes("Not found")
                $context.Response.OutputStream.Write($bytes, 0, $bytes.Length)
                $context.Response.Close()
                continue
            }

            $extension = [IO.Path]::GetExtension($filePath).ToLowerInvariant()
            if ($extension -eq ".json") {
                $context.Response.ContentType = "application/json"
            } else {
                $context.Response.ContentType = "application/octet-stream"
            }

            $bytes = [IO.File]::ReadAllBytes($filePath)
            $context.Response.ContentLength64 = $bytes.Length
            $context.Response.OutputStream.Write($bytes, 0, $bytes.Length)
            $context.Response.Close()
        }
    } finally {
        $listener.Stop()
        $listener.Close()
    }
}

try {
    Start-Sleep -Milliseconds 500
    if ($serverJob.State -ne "Running") {
        Receive-Job $serverJob
        throw "Local update feed did not start."
    }

    $env:HASHPDF_UPDATE_LATEST_URL = "$baseUrl/latest.json"
    $env:HASHPDF_UPDATE_RELEASE_URL_TEMPLATE = "$baseUrl/release-{tag}.json"

    Write-Host "Local update feed: $baseUrl"
    Write-Host "Demo install folder: $installRoot"
    Write-Host "Fake latest version: $Version"
    Write-Host ""
    Write-Host "HashPDF will open from the demo install folder. Choose Install in the update dialog to see the updater."
    Write-Host "Press Enter here after you finish testing to stop the local feed."

    Start-Process -FilePath (Join-Path $installRoot "HashPDF.exe") -WorkingDirectory $installRoot
    Read-Host | Out-Null
} finally {
    if ($serverJob) {
        Stop-Job $serverJob -ErrorAction SilentlyContinue
        Remove-Job $serverJob -Force -ErrorAction SilentlyContinue
    }

    Remove-Item Env:\HASHPDF_UPDATE_LATEST_URL -ErrorAction SilentlyContinue
    Remove-Item Env:\HASHPDF_UPDATE_RELEASE_URL_TEMPLATE -ErrorAction SilentlyContinue
}
