#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LoggingApp/LoggingApp.csproj", "LoggingApp/"]
RUN dotnet restore "./LoggingApp/LoggingApp.csproj"
COPY . .
WORKDIR "/src/LoggingApp"
RUN dotnet build "./LoggingApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
COPY ./../songswap_ssl.pfx ./songswap_ssl.pfx
RUN dotnet dev-certs https -ep ./songswap_ssl.pfx -p 411242
RUN dotnet publish "./LoggingApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoggingApp.dll"]