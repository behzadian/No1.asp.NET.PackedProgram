docker rm -f pp__os_engine
docker rm -f pp__os_dashboards
docker rm -f pp__pgdb
docker rm -f pp__pgmg
docker rm -f pp__fwy
docker rm -f pp__kc
docker rm -f pp__hc
docker rm -f pp__api

docker ps -a --filter volume=pp__pg_vl -q | ForEach-Object { docker rm -f $_ }
docker volume rm pp__pg_vl

docker ps -a --filter volume=pp__os_vl -q | ForEach-Object { docker rm -f $_ }
docker volume rm pp__os_vl

docker ps -a --filter volume=pp__hc_vl -q | ForEach-Object { docker rm -f $_ }
docker volume rm pp__hc_vl

$net="pp__net"
docker network inspect $net --format '{{range $id, $c := .Containers}}{{println $id}}{{end}}' |
ForEach-Object { docker rm -f $_ }
docker network rm $net

Write-Output "RUNNING DOCKER CONTAINERS:"
docker ps -a --format "{{.Names}}"