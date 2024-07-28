docker buildx build `
	--file '.\Dockerfile' `
	--platform 'linux/arm64,linux/amd64' `
	--pull `
	--push `
	--secret "id=ca_crt,src=${env:userprofile}\.aspnet\https\ca.crt" `
	--tag 'eassbhhtgu/asndenier:latest' `
	.
