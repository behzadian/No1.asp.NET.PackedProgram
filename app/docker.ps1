Remove-Item -Path ".nuget" -Recurse -Force
mkdir .nuget
copy $env:NUGET_HOME/* .nuget/
docker build --load -t farabank .
