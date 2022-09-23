docker pull mcr.microsoft.com/dotnet/sdk:7.0
if (!$?) { return; }

docker pull mcr.microsoft.com/dotnet/runtime:7.0
if (!$?) { return; }

docker build `
	--secret id=ca_crt,src=${env:userprofile}\.aspnet\https\ca.crt `
	--tag eassbhhtgu/asndenier:latest `
	.
if (!$?) { return; }

docker push eassbhhtgu/asndenier:latest
