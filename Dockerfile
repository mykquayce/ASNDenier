FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-env

ARG TARGETARCH
RUN --mount=type=secret,id=ca_crt,dst=/usr/local/share/ca-certificates/ca.crt \
	/usr/sbin/update-ca-certificates
WORKDIR /app
COPY . .
RUN dotnet restore ./ASNDenier.WorkerService/ASNDenier.WorkerService.csproj --arch $TARGETARCH --source https://api.nuget.org/v3/index.json --source https://nuget/v3/index.json
RUN dotnet publish ./ASNDenier.WorkerService/ASNDenier.WorkerService.csproj --arch $TARGETARCH --configuration Release --no-restore --output /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["./ASNDenier.WorkerService"]
