function log([string] $message) {
	$now = Get-Date -Format "hh:mm:ss"
	echo "$now : $message"
}

$ok = $true

$image = 'docker.io/eassbhhtgu/asndenier:latest'
docker pull --quiet $image
$imageCreated = docker inspect $image | jq --raw-output .[0].Created
log $imageCreated

$baseImages = `
	'mcr.microsoft.com/dotnet/runtime:10.0-alpine', `
	'mcr.microsoft.com/dotnet/sdk:10.0-alpine'

foreach ($baseImage in $baseImages) {
	docker pull --quiet $baseImage
	$baseImageCreated = docker inspect $baseImage | jq --raw-output .[0].Created
	log "$baseImageCreated : $baseImage"

	if ($imageCreated -lt $baseImageCreated) {
		$ok = $false
	}
}

if ( $ok ) {
	log 'up to date'
} else {
	log "rebuilding $image"
	docker buildx build `
		--file ./Dockerfile `
		--platform 'linux/arm64,linux/amd64' `
		--pull `
		--push `
		--tag 'eassbhhtgu/asndenier:latest' `
		.
}
