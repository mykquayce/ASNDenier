docker pull mcr.microsoft.com/dotnet/sdk:8.0
if (!$?) { return; }

docker pull mcr.microsoft.com/dotnet/runtime:8.0
if (!$?) { return; }

$secret = 'id=ca_crt,src={0}\.aspnet\https\ca.crt' -f ${env:userprofile}
docker build `
	--secret $secret `
	--tag eassbhhtgu/asndenier:latest `
	.
if (!$?) { return; }

docker push eassbhhtgu/asndenier:latest
