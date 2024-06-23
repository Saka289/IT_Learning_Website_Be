FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LW.API/LW.API.csproj", "LW.API/"]
COPY ["BuildingBlocks/LW.Infrastructure/LW.Infrastructure.csproj", "BuildingBlocks/LW.Infrastructure/"]
COPY ["BuildingBlocks/LW.Contracts/LW.Contracts.csproj", "BuildingBlocks/LW.Contracts/"]
COPY ["BuildingBlocks/LW.Shared/LW.Shared.csproj", "BuildingBlocks/LW.Shared/"]
COPY ["BuildingBlocks/LW.Logging/LW.Logging.csproj", "BuildingBlocks/LW.Logging/"]
COPY ["Libraries/LW.Cache/LW.Cache.csproj", "Libraries/LW.Cache/"]
COPY ["Libraries/LW.Data/LW.Data.csproj", "Libraries/LW.Data/"]
COPY ["Libraries/LW.Services/LW.Services.csproj", "Libraries/LW.Services/"]
RUN dotnet restore "LW.API/LW.API.csproj"
COPY . .
WORKDIR "/src/LW.API"
RUN dotnet build "LW.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LW.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LW.API.dll"]
