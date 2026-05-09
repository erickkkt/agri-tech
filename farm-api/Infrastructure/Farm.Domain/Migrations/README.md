# Migrations folder

The previous SQL Server migrations have been removed because Postgres column
types (`uuid`, `text`, `timestamp with time zone`, ...) differ from SQL Server
(`uniqueidentifier`, `nvarchar(max)`, `datetime2`, ...).

Regenerate from scratch after the provider swap:

```bash
cd farm-api
dotnet tool restore   # if you use a tool manifest, otherwise:
# dotnet tool install --global dotnet-ef --version 9.0.7

# from the solution root
dotnet ef migrations add InitFarmDb \
  --project Infrastructure/Farm.Domain \
  --startup-project Services/Farm.Api

dotnet ef database update \
  --project Infrastructure/Farm.Domain \
  --startup-project Services/Farm.Api
```

The Hangfire schema (`hangfire`) is created automatically at startup by
`Hangfire.PostgreSql` — it is not part of EF migrations.
