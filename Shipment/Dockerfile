#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Shipment/Shipment.csproj", "Shipment/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "Shipment/Shipment.csproj"
COPY . .
WORKDIR "/src/Shipment"
RUN dotnet build "Shipment.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Shipment.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shipment.dll"]