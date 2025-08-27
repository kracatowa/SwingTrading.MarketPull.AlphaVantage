# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

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

ENTRYPOINT ["dotnet", "AlphaVantage.MarketPull.dll"]