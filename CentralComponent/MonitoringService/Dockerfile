# The context for this Dockerfile is ../.. (two folders outside)

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app

# Copy the project file and restore any dependencies
COPY ["CentralComponent/MonitoringService/MonitoringService.csproj", "CentralComponent/MonitoringService/"]
RUN dotnet restore "CentralComponent/MonitoringService/MonitoringService.csproj"

# Copy the remaining source code and build the application
COPY . .
COPY StorageMiddleware/StorageConnector/ StorageMiddleware/StorageConnector/
RUN dotnet publish "CentralComponent/MonitoringService/MonitoringService.csproj" -c Release -o /app/publish

WORKDIR /app/publish

EXPOSE 80
EXPOSE 5005

ENTRYPOINT ["dotnet", "MonitoringService.dll"]