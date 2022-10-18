#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CMS_App_Api/CMS_App_Api.csproj", "CMS_App_Api/"]
COPY ["CMS_Access/CMS_Access.csproj", "CMS_Access/"]
COPY ["CMS_EF/CMS_EF.csproj", "CMS_EF/"]
COPY ["CMS_Lib/CMS_Lib.csproj", "CMS_Lib/"]
COPY ["CMS_Ship/CMS_Ship.csproj", "CMS_Ship/"]
COPY ["CMS_WareHouse/CMS_WareHouse.csproj", "CMS_WareHouse/"]
RUN dotnet restore "CMS_App_Api/CMS_App_Api.csproj"
COPY . .
WORKDIR "/src/CMS_App_Api"
RUN dotnet build "CMS_App_Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMS_App_Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMS_App_Api.dll"]