# Use the .NET SDK image for building and running the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /src
EXPOSE 80
EXPOSE 5005

# Install the .NET Core debugger (vsdbg)
RUN apt-get update && apt-get install -y unzip curl && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ARG configuration=Release

# Copy the project file and restore dependencies
COPY ["StorageMiddleware/StorageMiddlewareApi/StorageMiddlewareApi.csproj", "StorageMiddleware/StorageMiddlewareApi/"]
RUN dotnet restore "StorageMiddleware/StorageMiddlewareApi/StorageMiddlewareApi.csproj"

# Copy all the source code and build the application
COPY . .
# Copy and restore all dependent projects
COPY StorageMiddleware/CacheProvider StorageMiddleware/CacheProvider
COPY StorageMiddleware/StorageAdapters StorageMiddleware/StorageAdapters

# Publish the application to the publish directory
RUN dotnet publish "StorageMiddleware/StorageMiddlewareApi/StorageMiddlewareApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

# Set the working directory to the publish folder and run the application
WORKDIR /app/publish
ENTRYPOINT ["dotnet", "StorageMiddlewareApi.dll"]
