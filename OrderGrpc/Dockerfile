#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["OrderGrpc/OrderGrpc.csproj", "OrderGrpc/"]
COPY ["OrderCommon/OrderCommon.csproj", "OrderCommon/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "OrderGrpc/OrderGrpc.csproj"
COPY . .
WORKDIR "/src/OrderGrpc"
RUN dotnet build "OrderGrpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderGrpc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderGrpc.dll"]