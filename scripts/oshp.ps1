# -----------------------------
# Resolve paths safely
# -----------------------------
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

$envFile   = Join-Path $scriptDir "../.env"
$usersFile = Join-Path $scriptDir "../dc/os/engine/os_eg_users.yml"
$usersFile = [System.IO.Path]::GetFullPath($usersFile)

Write-Host "ENV file   : $envFile"
Write-Host "TARGET file: $usersFile"

# -----------------------------
# Ensure global zxcvbn (idempotent)
# -----------------------------
function Ensure-ZxcvbnGlobal {
    $check = npm list -g zxcvbn --depth=0 2>$null

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Installing zxcvbn globally..."
        npm install -g zxcvbn | Out-Null
    }
}

Ensure-ZxcvbnGlobal

# -----------------------------
# Locate global node modules path
# -----------------------------
$npmRoot = npm root -g
$env:NODE_PATH = $npmRoot

Write-Host "Using global NODE_PATH: $npmRoot"

# -----------------------------
# Ensure helper script exists (idempotent)
# -----------------------------
$helperDir  = Join-Path $scriptDir ".tools"
$helperFile = Join-Path $helperDir "zxcvbn-check.js"

if (-not (Test-Path $helperDir)) {
    New-Item -ItemType Directory -Path $helperDir | Out-Null
}

if (-not (Test-Path $helperFile)) {

@"
const zxcvbn = require('zxcvbn');

let input = '';
process.stdin.setEncoding('utf8');

process.stdin.on('data', chunk => input += chunk);
process.stdin.on('end', () => {
    const result = zxcvbn(input.trim());
    process.stdout.write(result.score.toString());
});
"@ | Set-Content -Path $helperFile -Encoding UTF8

}

# -----------------------------
# Secure password check (stdin, no leakage)
# -----------------------------
function Test-PasswordStrength {
    param(
        [string]$Password,
        [string]$HelperFile
    )

    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = "node"
    $psi.Arguments = "`"$HelperFile`""
    $psi.RedirectStandardInput = $true
    $psi.RedirectStandardOutput = $true
    $psi.RedirectStandardError = $true
    $psi.UseShellExecute = $false
    $psi.CreateNoWindow = $true
    $psi.EnvironmentVariables["NODE_PATH"] = $env:NODE_PATH

    $process = [System.Diagnostics.Process]::Start($psi)

    $process.StandardInput.Write($Password)
    $process.StandardInput.Close()

    $score = $process.StandardOutput.ReadToEnd().Trim()
    $process.WaitForExit()

    return [int]$score
}

# -----------------------------
# Read password from .env
# -----------------------------
$passwordLine = Get-Content $envFile |
    Where-Object { $_ -match '^OPENSEARCH_INITIAL_ADMIN_PASSWORD=' } |
    Select-Object -First 1

if (-not $passwordLine) {
    throw "OPENSEARCH_INITIAL_ADMIN_PASSWORD not found in $envFile"
}

$password = ($passwordLine -split '=', 2)[1].Trim()

# -----------------------------
# Validate strength
# -----------------------------
$score = Test-PasswordStrength -Password $password -HelperFile $helperFile

Write-Host "Password strength score: $score"

if ($score -ne 4) {
    throw "Password is too weak (zxcvbn score: $score). Required score is 4."
}

#================================

$password = $passwordLine -replace '^OPENSEARCH_INITIAL_ADMIN_PASSWORD=', ''
$password = $password.Trim()

# remove optional quotes
if (
    ($password.StartsWith('"') -and $password.EndsWith('"')) -or
    ($password.StartsWith("'") -and $password.EndsWith("'"))
) {
    $password = $password.Substring(1, $password.Length - 2)
}

Write-Host "Generating OpenSearch `$password` hash..."

# -----------------------------
# Generate hash
# -----------------------------
$hash = docker run --rm opensearchproject/opensearch:latest `
  /usr/share/opensearch/plugins/opensearch-security/tools/hash.sh -p "$password"

if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($hash)) {
    throw "Failed to generate password hash."
}

$hash = $hash.Trim()

Write-Host "Generated hash: $hash"

# -----------------------------
# Read YAML file
# -----------------------------
$content = Get-Content $usersFile -Raw

$match = [regex]::Match($content, '(?m)^(\s*hash:\s*)(.+)$')

if (-not $match.Success) {
    throw "admin.hash not found in YAML"
}

$currentHash = $match.Groups[2].Value.Trim()

Write-Host "Current hash : $currentHash"

# -----------------------------
# Replace ONLY hash line
# -----------------------------
$updatedContent = [regex]::Replace(
    $content,
    '(?m)^(\s*hash:\s*).+$',
    "`$1$hash"
)

# -----------------------------
# Write file
# -----------------------------
Set-Content -Path $usersFile -Value $updatedContent -NoNewline -Encoding UTF8

# -----------------------------
# VERIFY IN MEMORY ONLY (NO DISK READ)
# -----------------------------
$verifyMatch = [regex]::Match($updatedContent, '(?m)^(\s*hash:\s*)(.+)$')

if (-not $verifyMatch.Success) {
    throw "Verification failed: hash missing after update"
}

$writtenHash = $verifyMatch.Groups[2].Value.Trim()

if ($writtenHash -notmatch '^\$2y\$|\$argon2|\$bcrypt') {
    throw "Corrupted hash detected: $writtenHash"
}

if ($writtenHash -ne $hash) {
    throw @"
Hash mismatch after update.

Expected:
$hash

Found:
$writtenHash
"@
}

# -----------------------------
# Final status
# -----------------------------
if ($currentHash -eq $hash) {
    Write-Host "Hash already up to date (no change needed)."
} else {
    Write-Host "Hash updated successfully."
}