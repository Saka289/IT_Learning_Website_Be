FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#####################
#PUPPETEER RECIPE
#####################
RUN apt-get update && apt-get -f install && apt-get -y install wget gnupg2 apt-utils
RUN wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN echo 'deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main' >> /etc/apt/sources.list
RUN apt-get update \
&& apt-get install -y google-chrome-stable --no-install-recommends --allow-downgrades fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf
######################
#END PUPPETEER RECIPE
######################
#####################
#Aspose.PDF RECIPE
#####################
RUN apt-get update && apt-get install -y libgdiplus
RUN sed -i'.bak' 's/$/ contrib/' /etc/apt/sources.list
RUN apt-get update; apt-get install -y ttf-mscorefonts-installer fontconfig
######################
#END Aspose.PDF RECIPE
######################
ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome-stable"
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LW.API/LW.API.csproj", "LW.API/"]
COPY ["BuildingBlocks/LW.Logging/LW.Logging.csproj", "BuildingBlocks/LW.Logging/"]
COPY ["BuildingBlocks/LW.Shared/LW.Shared.csproj", "BuildingBlocks/LW.Shared/"]
COPY ["Libraries/LW.Cache/LW.Cache.csproj", "Libraries/LW.Cache/"]
COPY ["BuildingBlocks/LW.Contracts/LW.Contracts.csproj", "BuildingBlocks/LW.Contracts/"]
COPY ["Libraries/LW.Data/LW.Data.csproj", "Libraries/LW.Data/"]
COPY ["BuildingBlocks/LW.Infrastructure/LW.Infrastructure.csproj", "BuildingBlocks/LW.Infrastructure/"]
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

