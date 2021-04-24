FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /app
COPY . .
RUN dotnet restore ASNDenier.sln --source https://api.nuget.org/v3/index.json --source http://nuget/v3/index.json
RUN dotnet publish ASNDenier.WorkerService/ASNDenier.WorkerService.csproj --configuration Release --output /app/publish --runtime linux-x64

FROM mcr.microsoft.com/dotnet/aspnet:6.0
ENV DOTNET_ENVIRONMENT=Production
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "ASNDenier.WorkerService.dll"]
