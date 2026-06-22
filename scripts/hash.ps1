$ErrorActionPreference = 'Stop'

if ($MyInvocation.InvocationName -eq '.') {
    Write-Error 'ERROR: Do not dot-source this script. Use: ./hash.ps1'
    exit 1
}

$rootDir = (Resolve-Path (Join-Path $PSScriptRoot '../app')).Path

Write-Host "Root directory: $rootDir"
Write-Host ''
Write-Host '=== Files in hashing order ==='
Write-Host ''

$fileList = [System.Collections.Generic.List[string]]::new()

Get-ChildItem -Path $rootDir -Recurse -File | Where-Object {
    $fullName = $_.FullName
    ($fullName -notmatch '[/\\]bin[/\\]' -and $fullName -notmatch '[/\\]obj[/\\]') -and
    ($_.Extension -eq '.cs' -or $_.Extension -eq '.csproj' -or $_.Name -like 'appsettings*.json')
} | ForEach-Object { $fileList.Add($_.FullName) }

Get-ChildItem -Path $rootDir -Depth 0 -File -Filter '*.slnx' | ForEach-Object {
    $fileList.Add($_.FullName)
}

Write-Host "Found $($fileList.Count) matching files"
Write-Host ''

$tempFile = [System.IO.Path]::GetTempFileName()
$hashLines = [System.Collections.Generic.List[string]]::new()

try {
    $sortedFiles = $fileList.ToArray()
    [System.Array]::Sort($sortedFiles, [StringComparer]::Ordinal)

    $sortedFiles | ForEach-Object {
        $file = $_
        $filename = [System.IO.Path]::GetFileName($file)
        $hash = (Get-FileHash -Path $file -Algorithm SHA256).Hash.ToLowerInvariant()
        $relativePath = $file.Substring($rootDir.Length + 1) -replace '\\', '/'

        Write-Host "HASH: $hash"
        Write-Host "FILE: $filename"
        Write-Host "PATH: $relativePath"
        Write-Host '--------------------------------------------------'

        $hashLines.Add($hash)
    }

    [System.IO.File]::WriteAllText($tempFile, ($hashLines -join "`n") + "`n", [System.Text.UTF8Encoding]::new($false))

    Write-Host ''
    Write-Host '=== Generating final hash ==='

    $finalHash = (Get-FileHash -Path $tempFile -Algorithm SHA256).Hash.ToLowerInvariant().Substring(0, 48)

    Write-Host "Final combined hash: $finalHash"
    Write-Host "BUILD_HASH=$finalHash"
}
finally {
    Remove-Item -Path $tempFile -Force -ErrorAction SilentlyContinue
}
