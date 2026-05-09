# Agri-Tech - Farm Management Platform

Monorepo for the **Agri-Tech** farm management platform.

The platform delivers three core experiences:

1. **Farm Management** - Track animals, vaccines, feed inventory, growth logs, diseases, and receive automatic alerts.
2. **Marketplace** - Connect farms, post listings (breeding stock, antler/velvet, meat, eggs), forum, reviews, shipping partners.
3. **Investment Platform (Chăn nuôi 4.0)** - Allow city investors to buy share-based ownership of real animals, follow them via daily logs and camera feeds, and receive profit at harvest.

## Repository layout

```
agri-tech/
├── farm-api/      # ASP.NET Core 8 backend (REST + SignalR + Hangfire)
├── farm-admin/    # Angular 18 admin app for farm owners / staff / vets
├── farm-user/     # Angular 18 end-user app for buyers / investors
├── docs/          # Design docs, test cases, changelogs
└── docker-compose.yml  # Orchestrates the full stack for local dev
```

## Quick start

### 1. Local development without Docker

```bash
# Backend
cd farm-api
dotnet restore
dotnet ef database update -p Infrastructure/Farm.Domain/Farm.Domain.csproj -s Services/Farm.Api/Farm.Api.csproj
dotnet run --project Services/Farm.Api/Farm.Api.csproj

# Admin app (in a new terminal)
cd farm-admin
npm install
npm start            # http://localhost:4200

# User app (in a new terminal)
cd farm-user
npm install
npm start            # http://localhost:4300 (configure in angular.json)
```

### 2. Docker Compose - full stack in one command

```bash
docker compose up --build
```

This starts:
- `sqlserver` on port `1433`
- `farm-api` on port `8080` (Swagger at `/swagger`, Hangfire dashboard at `/hangfire`, SignalR at `/hubs/notifications`)
- `farm-admin` on port `4200`
- `farm-user` on port `4300`

## Documentation

- [Design overview (DOCX)](docs/01_design_overview.docx) - architecture, ERD, API spec, role matrix.
- [Test cases (XLSX)](docs/02_test_cases.xlsx) - 81 QA cases across all phases.
- [Changelog v1](docs/00_CHANGELOG_v1.md) - what was added, where, and why.

## Tech stack

| Layer | Tech |
|-------|------|
| Backend | ASP.NET Core 8, EF Core 9, SQL Server, Hangfire, SignalR |
| Admin & User UI | Angular 18, Angular Material, OAuth2/OIDC |
| Realtime | SignalR (`@microsoft/signalr` on the client) |
| Background jobs | Hangfire (vaccine reminders, weight-drop detection, low-stock, daily report) |
| Auth | JWT Bearer (Azure AD / B2C compatible) |
| Storage | Local file (dev) / Azure Blob (prod) for animal photos, contracts, camera snapshots |

## License

Proprietary - all rights reserved.
