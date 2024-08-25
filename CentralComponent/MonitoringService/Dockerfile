# The context for this Dockerfile is ../.. (two folders outside)

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /src

# Install the .NET Core debugger (vsdbg)
RUN apt-get update && apt-get install -y unzip curl && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

# Copy the project file and restore any dependencies
COPY ["CentralComponent/MonitoringService/MonitoringService.csproj", "CentralComponent/MonitoringService/"]
RUN dotnet restore "CentralComponent/MonitoringService/MonitoringService.csproj"

# Copy the remaining source code and build the application
COPY . .
COPY StorageConnector/ StorageConnector/
RUN dotnet publish "CentralComponent/MonitoringService/MonitoringService.csproj" -c Release -o /app/publish

WORKDIR /app/publish

EXPOSE 80
EXPOSE 5005

ENTRYPOINT ["dotnet", "MonitoringService.dll"]