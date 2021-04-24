FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /app
COPY . .
RUN dotnet restore ASNBlacklister.sln --source https://api.nuget.org/v3/index.json --source http://nuget/v3/index.json
RUN dotnet publish ASNBlacklister.WorkerService/ASNBlacklister.WorkerService.csproj --configuration Release --output /app/publish --runtime linux-x64

FROM mcr.microsoft.com/dotnet/aspnet:6.0
ENV DOTNET_ENVIRONMENT=Production
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "ASNBlacklister.WorkerService.dll"]
