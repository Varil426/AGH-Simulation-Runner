#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SimulationRunnerService/SimulationRunnerService.csproj", "SimulationRunnerService/"]
COPY ["SimulationHandler/SimulationHandler.csproj", "SimulationHandler/"]
COPY ["SimulationStandard/SimulationStandard.csproj", "SimulationStandard/"]
RUN dotnet restore "SimulationRunnerService/SimulationRunnerService.csproj"
RUN dotnet restore "SimulationHandler/SimulationHandler.csproj"
RUN dotnet restore "SimulationStandard/SimulationStandard.csproj"
WORKDIR "/src/SimulationHandler"
COPY SimulationHandler .
WORKDIR "/src/SimulationStandard"
COPY SimulationStandard .
WORKDIR "/src/SimulationRunnerService"
COPY SimulationRunnerService .
RUN dotnet build "SimulationRunnerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimulationRunnerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimulationRunnerService.dll"]