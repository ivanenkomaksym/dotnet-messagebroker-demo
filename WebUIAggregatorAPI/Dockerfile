#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["WebUIAggregatorAPI/WebUIAggregatorAPI.csproj", "WebUIAggregatorAPI/"]
COPY ["CatalogAPI/CatalogAPI.csproj", "CatalogAPI/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "WebUIAggregatorAPI/WebUIAggregatorAPI.csproj"
COPY . .
WORKDIR "/src/WebUIAggregatorAPI"
RUN dotnet build "WebUIAggregatorAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebUIAggregatorAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebUIAggregatorAPI.dll"]