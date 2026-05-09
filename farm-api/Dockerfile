# =====================================================================
# farm-api - .NET 8 Web API (PostgreSQL via Npgsql + Hangfire.PostgreSql)
# Multi-stage build: restore → build → publish → runtime
# =====================================================================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
# Postgres needs ICU at runtime for case-insensitive collations and globalization-aware string comparisons
RUN apt-get update \
    && apt-get install -y --no-install-recommends libicu-dev tzdata \
    && rm -rf /var/lib/apt/lists/*
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=false

# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all csproj files first so `dotnet restore` is cached when only source code changes.
# Farm.Api references Farm.Business which references Farm.Domain — restore needs the full graph.
COPY ["Farm.sln", "./"]
COPY ["Services/Farm.Api/Farm.Api.csproj",                       "Services/Farm.Api/"]
COPY ["Infrastructure/Farm.Business/Farm.Business.csproj",       "Infrastructure/Farm.Business/"]
COPY ["Infrastructure/Farm.Domain/Farm.Domain.csproj",           "Infrastructure/Farm.Domain/"]
COPY ["Tests/Farm.TestApi/Farm.TestApi.csproj",                  "Tests/Farm.TestApi/"]
COPY ["Tests/Farm.TestBusiness/Farm.TestBusiness.csproj",        "Tests/Farm.TestBusiness/"]
COPY ["Tests/TestDomain/Farm.TestDomain.csproj",                 "Tests/TestDomain/"]

RUN dotnet restore "Services/Farm.Api/Farm.Api.csproj"

# Copy the rest of the source and build
COPY . .
WORKDIR "/src/Services/Farm.Api"
RUN dotnet build "Farm.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

# ---------- publish ----------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Farm.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

# ---------- runtime ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Farm.Api.dll"]
