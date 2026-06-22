$ErrorActionPreference = 'Stop'

./decompose.ps1

docker compose -f ./docker-compose.yml up -d
#docker logs -f pp__api
