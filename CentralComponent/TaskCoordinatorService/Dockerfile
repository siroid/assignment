# The context for this Dockerfile is ../.. (two folders outside)

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app

# Copy the project file and restore any dependencies
COPY ["CentralComponent/TaskCoordinatorService/TaskCoordinatorService.csproj", "CentralComponent/TaskCoordinatorService/"]
RUN dotnet restore "CentralComponent/TaskCoordinatorService/TaskCoordinatorService.csproj"

# Copy the remaining source code and build the application
COPY . .
COPY StorageMiddleware/StorageConnector/ StorageMiddleware/StorageConnector/
RUN dotnet publish "CentralComponent/TaskCoordinatorService/TaskCoordinatorService.csproj" -c Release -o /app/publish

WORKDIR /app/publish
EXPOSE 80
EXPOSE 5005

ENTRYPOINT ["dotnet", "TaskCoordinatorService.dll"]