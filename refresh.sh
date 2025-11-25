#! /bin/bash

function log() {
	echo "$(date -u +%X) : $1"
}

ok=true

image='docker.io/eassbhhtgu/asndenier:latest'
docker pull --quiet $image
imageCreated=$( docker inspect $image | jq --raw-output .[0].Created )
log "$imageCreated"

baseImages=( \
	'mcr.microsoft.com/dotnet/runtime:10.0-alpine' \
	'mcr.microsoft.com/dotnet/sdk:10.0-alpine' )

for baseImage in ${baseImages[@]}; do
	docker pull --quiet $baseImage
	baseImageCreated=$( docker inspect $baseImage | jq --raw-output .[0].Created )
	log "$baseImageCreated: $baseImage"

	if [ $imageCreated < $baseImageCreated ]; then
		$ok = false
	fi
done

if [ $ok ]; then
	log 'up to date'
else
	log "rebuilding $image"
	docker buildx build \
		--file ./Dockerfile \
		--platform 'linux/arm64,linux/amd64' \
		--pull \
		--push \
		--tag 'eassbhhtgu/asndenier:latest' \
		.
fi
