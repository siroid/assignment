# Use the .NET SDK image for building and running the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5005

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ARG configuration=Release

# Copy the project file and restore dependencies
WORKDIR /src
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
