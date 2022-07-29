FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

RUN --mount=type=secret,id=ca_crt,dst=/usr/local/share/ca-certificates/ca.crt \
	/usr/sbin/update-ca-certificates
WORKDIR /app
COPY . .
RUN dotnet restore ASNDenier.sln --source https://api.nuget.org/v3/index.json --source https://nuget/v3/index.json
RUN dotnet publish ASNDenier.WorkerService/ASNDenier.WorkerService.csproj --configuration Release --output /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0
ENV DOTNET_ENVIRONMENT=Production
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "ASNDenier.WorkerService.dll"]
