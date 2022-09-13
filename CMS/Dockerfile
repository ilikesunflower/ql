#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 7600
EXPOSE 7500

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CMS/CMS.csproj", "CMS/"]
COPY ["CMS_Access/CMS_Access.csproj", "CMS_Access/"]
COPY ["CMS_EF/CMS_EF.csproj", "CMS_EF/"]
COPY ["CMS_Lib/CMS_Lib.csproj", "CMS_Lib/"]
COPY ["CMS_Ship/CMS_Ship.csproj", "CMS_Ship/"]
COPY ["CMS_WareHouse/CMS_WareHouse.csproj", "CMS_WareHouse/"]
RUN dotnet restore "CMS/CMS.csproj"
COPY . .
WORKDIR "/src/CMS"
RUN dotnet build "CMS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMS.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMS.dll"]