docker rm -f fb__os_engine
docker rm -f fb__os_dashboards
docker rm -f fb__pgdb
docker rm -f fb__pgmg
docker rm -f fb__fwy
docker rm -f fb__kc
docker rm -f fb__hc
docker rm -f fb__api

docker ps -a --filter volume=fb__pg_vl -q | ForEach-Object { docker rm -f $_ }
docker volume rm fb__pg_vl

docker ps -a --filter volume=fb__os_vl -q | ForEach-Object { docker rm -f $_ }
docker volume rm fb__os_vl

docker ps -a --filter volume=fb__hc_vl -q | ForEach-Object { docker rm -f $_ }
docker volume rm fb__hc_vl

$net="fb__net"
docker network inspect $net --format '{{range $id, $c := .Containers}}{{println $id}}{{end}}' |
ForEach-Object { docker rm -f $_ }
docker network rm $net

docker ps -a --format "{{.Names}}"