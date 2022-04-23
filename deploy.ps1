docker pull eassbhhtgu/asnblacklister:latest
if (!$?) { return; }

docker stack deploy --compose-file .\docker-compose.yml asnblacklister
