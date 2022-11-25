if ( git status --short ) {
	echo "pending changes"
	exit 0
}

docker pull mcr.microsoft.com/dotnet/runtime:7.0
docker pull mcr.microsoft.com/dotnet/sdk:7.0
docker pull eassbhhtgu/asndenier:latest

$base1 = docker image inspect --format '{{.Created}}' mcr.microsoft.com/dotnet/runtime:7.0
$base2 = docker image inspect --format '{{.Created}}' mcr.microsoft.com/dotnet/sdk:7.0
$target = docker image inspect --format '{{.Created}}' eassbhhtgu/asndenier:latest

if ($base1 -gt $target -or $base2 -gt $target) {

	git clean -dfx
	git fetch
	git pull

	$secret = 'id=ca_crt,src={0}\.aspnet\https\ca.crt' -f ${env:userprofile}
	docker build `
	--secret $secret `
		--tag eassbhhtgu/asndenier:latest `
		.
	if (!$?) { exit 1; }

	docker push eassbhhtgu/asndenier:latest
	if (!$?) { exit 1; }
}
