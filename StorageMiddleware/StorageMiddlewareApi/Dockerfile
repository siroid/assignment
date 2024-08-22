FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StorageMiddlewareApi/StorageMiddlewareApi.csproj", "StorageMiddlewareApi/"]
RUN dotnet restore "StorageMiddlewareApi/StorageMiddlewareApi.csproj"
COPY . .
WORKDIR "/src/StorageMiddlewareApi"
RUN dotnet build "StorageMiddlewareApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StorageMiddlewareApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StorageMiddlewareApi.dll"]
