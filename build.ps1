docker buildx build `
	--file '.\Dockerfile' `
	--platform 'linux/arm64,linux/amd64' `
	--pull `
	--push `
	--tag 'eassbhhtgu/asndenier:latest' `
	.
