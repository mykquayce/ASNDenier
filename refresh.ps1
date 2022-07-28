docker pull mcr.microsoft.com/dotnet/runtime:6.0
docker pull mcr.microsoft.com/dotnet/sdk:6.0
docker pull eassbhhtgu/asndenier:latest

$base1 = docker image inspect --format '{{.Created}}' mcr.microsoft.com/dotnet/runtime:6.0
$base2 = docker image inspect --format '{{.Created}}' mcr.microsoft.com/dotnet/sdk:6.0
$target = docker image inspect --format '{{.Created}}' eassbhhtgu/asndenier:latest

if ($base1 -gt $target -or $base2 -gt $target) {

	docker build `
		--secret id=ca_crt,src=${env:userprofile}\.aspnet\https\ca.crt `
		--tag eassbhhtgu/asndenier:latest `
		.
	if (!$?) { exit 1; }

	docker push eassbhhtgu/asndenier:latest
	if (!$?) { exit 1; }
}
