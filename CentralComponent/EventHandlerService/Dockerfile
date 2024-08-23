# The context for this Dockerfile is ../.. (two folders outside)

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app

# Install the .NET Core debugger (vsdbg)
RUN apt-get update && apt-get install -y unzip curl && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

# Copy the project file and restore any dependencies
COPY ["CentralComponent/EventHandlerService/EventHandlerService.csproj", "CentralComponent/EventHandlerService/"]
RUN dotnet restore "CentralComponent/EventHandlerService/EventHandlerService.csproj"

# Copy the remaining source code and build the application
COPY . .
COPY StorageMiddleware/StorageConnector/ StorageMiddleware/StorageConnector/
RUN dotnet publish "CentralComponent/EventHandlerService/EventHandlerService.csproj" -c Release -o /app/publish

WORKDIR /app/publish

# For remote debugging
EXPOSE 9229  

ENTRYPOINT ["dotnet", "EventHandlerService.dll"]