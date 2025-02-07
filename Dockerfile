FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build-env

ARG TARGETARCH
WORKDIR /app
COPY . .
RUN dotnet restore ./ASNDenier.WorkerService/ASNDenier.WorkerService.csproj --arch $TARGETARCH --source https://api.nuget.org/v3/index.json --source https://nuget.bob.house/v3/index.json
RUN dotnet publish ./ASNDenier.WorkerService/ASNDenier.WorkerService.csproj --arch $TARGETARCH --configuration Release --no-restore --output /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["./ASNDenier.WorkerService"]
