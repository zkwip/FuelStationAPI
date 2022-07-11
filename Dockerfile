# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-bionic AS build
WORKDIR /source
COPY . .
RUN dotnet restore "./FuelStationAPI/FuelStationAPI.csproj" -disable-parallel
RUN dotnet publish "./FuelStationAPI/FuelStationAPI.csproj" -c release -p /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-bionic AS build
WORKDIR /app
COPY --from=build /app ./

EXPOSE 5000
ENTRYPOINT ["dotnet","FuelStationAPI.dll"]
