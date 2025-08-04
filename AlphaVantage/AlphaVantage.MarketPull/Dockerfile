# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

RUN apt-get update && apt-get install -y cron curl dos2unix && rm -rf /var/lib/apt/lists/*

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY AlphaVantage.MarketPull.csproj .
RUN dotnet restore "AlphaVantage.MarketPull.csproj"
COPY . .
RUN dotnet publish "AlphaVantage.MarketPull.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage for production
FROM base AS final
WORKDIR /app
RUN mkdir -p sharedstorage && chmod -R 777 sharedstorage
COPY --from=build /app/publish .
COPY /Pipelines/entrypoint.sh /entrypoint.sh
RUN dos2unix /entrypoint.sh
RUN chmod +x /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]