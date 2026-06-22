$ErrorActionPreference = 'Stop'

./decompose.ps1
./oshp.ps1

$output = & (Join-Path $PSScriptRoot 'hash.ps1') 2>&1
# print all script output
$output | ForEach-Object { Write-Host $_ }
# extract BUILD_HASH
$hashLine = $output | Select-String '^BUILD_HASH='
$env:BUILD_HASH = ($hashLine -split '=', 2)[1]

Write-Output "Copying nuget."
if ([string]::IsNullOrWhiteSpace($env:NUGET_HOME) -or -not (Test-Path -Path $env:NUGET_HOME -PathType Container)) {
    Write-Error "NUGET_HOME is not set or does not point to an existing directory."
    exit 1
}
Remove-Item -Path "../.nuget" -Recurse -Force  -ErrorAction SilentlyContinue
mkdir ../.nuget
Copy-Item $env:NUGET_HOME/* ../.nuget/
Write-Output "Nuget copied."

#docker compose -f ../docker-compose.yml down -v

Write-Output "Preparing init.sh"
$initSh = Join-Path $PSScriptRoot '../dc/pg/init.sh'
$text = [IO.File]::ReadAllText($initSh)
$text = $text -replace "`r`n", "`n"
[IO.File]::WriteAllText($initSh, $text)
Write-Output "init.sh prepared"

Write-Output "Building docker..."
docker compose -f ../docker-compose.yml build --progress=plain --no-cache --pull --build-arg BUILD_INSTANT=$(Get-Date -Format "yyyy-MM-dd HH:mm:ss") --build-arg BUILD_HASH=$env:BUILD_HASH
#docker compose -f ../docker-compose.yml build --progress=plain --build-arg BUILD_INSTANT=$(Get-Date -Format "yyyy-MM-dd HH:mm:ss") --build-arg BUILD_HASH=$env:BUILD_HASH
if ($LASTEXITCODE -ne 0) {
    throw "Docker build failed."
}
Write-Output "Docker built"

docker compose -f ../docker-compose.yml up -d
docker exec pp__os_engine /usr/share/opensearch/plugins/opensearch-security/tools/securityadmin.sh `
  -cd "/usr/share/opensearch/config/opensearch-security/" `
  -cacert /usr/share/opensearch/config/root-ca.pem `
  -cert /usr/share/opensearch/config/admin.pem `
  -key /usr/share/opensearch/config/admin-key.pem `
  -nhnv `
  -icl
Write-Output "Finish setting up security in OPENSEARCH"
#docker logs -f pp__api
