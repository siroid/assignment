FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install the .NET Core debugger (vsdbg)
RUN apt-get update && apt-get install -y unzip curl && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

# Copy the project file and restore any dependencies
COPY ["RoboticArm/RoboticArmService.csproj", "RoboticArm/"]
RUN dotnet restore "RoboticArm/RoboticArmService.csproj"

# Copy the remaining source code and build the application
COPY . .

RUN dotnet publish "RoboticArm/RoboticArmService.csproj" -c Release -o /app/publish

WORKDIR /app/publish

# Install the .NET Core debugger (vsdbg)
RUN curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg
# For remote debugging
EXPOSE 9229  

ENTRYPOINT ["dotnet", "RoboticArmService.dll"]

