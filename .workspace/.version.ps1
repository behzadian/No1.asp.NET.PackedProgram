# Global Hash calculator file
# Version 260604.1757
# Version 260604.1110
# Version 260604.1109
# Version 260604.1049

$files = @(
    "../.dockerignore",
    "../.editorconfig",
    "../.gitattributes",
    "../.gitignore",
    ".version.ps1",
    ".version.sh"
)

$timestamp = Get-Date -Format "yyMMdd.HHmm"

# Add version line as second line in each file
foreach ($f in $files) {
    if (Test-Path $f) {
        $content = Get-Content $f -Raw
        $lines = $content -split "`r`n|`n"
        $newContent = $lines[0] + "`n# Version $timestamp`n" + ($lines[1..$lines.Count] -join "`n")
        $newContent | Set-Content $f -Encoding UTF8 -NoNewline
    }
}

# Compute final hash
$enc = [System.Text.Encoding]::UTF8
$hashAlg = [System.Security.Cryptography.MD5]::Create()
$combined = ""

foreach ($f in ($files | Sort-Object)) {
    $raw = Get-Content $f -Raw

    $normalized = $raw -replace "`r`n", "`n" -replace "`r", "`n"

    $cleanLines = $normalized -split "`n" |
        Where-Object { $_ -notmatch '^\s*#' } |
        Where-Object { $_.Trim() -ne '' } |
        ForEach-Object { $_.Trim() }

    $cleanText = $cleanLines -join "`n"

    $bytes = $enc.GetBytes($cleanText)
    $fileHash = [Convert]::ToHexString($hashAlg.ComputeHash($bytes))

    $combined += $fileHash
}

# Final hash
$finalBytes = $enc.GetBytes($combined)
$finalHash = [Convert]::ToHexString($hashAlg.ComputeHash($finalBytes))

# Correct formatting: every 2 characters with underscore
$formatted = [regex]::Replace($finalHash, '(.{2})', '$1_').TrimEnd('_')

# Save result
"${timestamp}:${formatted}" | Add-Content ".version" -Encoding UTF8
Write-Host "PS : ${timestamp}:${formatted}"