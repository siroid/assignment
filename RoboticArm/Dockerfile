# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore any dependencies
COPY ["RoboticArmService.csproj", "./"]
RUN dotnet restore "./RoboticArmService.csproj"

# Copy the remaining source code and build the application
COPY . .
RUN dotnet build "RoboticArmService.csproj" -c Release -o /app/build

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish "RoboticArmService.csproj" -c Release -o /app/publish

# Stage 3: Run the application
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app

# Install the .NET Core debugger (vsdbg)
RUN curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

# Copy the published application to the final image
COPY --from=publish /app/publish .

# Expose any ports the application needs to communicate through, if applicable
# EXPOSE 80

# Set the entry point for the application
ENTRYPOINT ["dotnet", "RoboticArmService.dll"]

