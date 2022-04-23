docker pull eassbhhtgu/asndenier:latest
if (!$?) { return; }

docker stack deploy --compose-file .\docker-compose.yml asndenier
