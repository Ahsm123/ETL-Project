FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ETLConfig.API/ETLConfig.API.csproj ETLConfig.API/
COPY ETL.Domain/ETL.Domain.csproj ETL.Domain/

RUN dotnet restore ETLConfig.API/ETLConfig.API.csproj

COPY ETLConfig.API/ ETLConfig.API/
COPY ETL.Domain/ ETL.Domain/

# Build
WORKDIR /src/ETLConfig.API
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ETLConfig.API.dll"]
