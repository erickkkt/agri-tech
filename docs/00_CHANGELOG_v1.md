# Farm Management System - Phase 1 → Phase 3 Changelog

This document summarizes all changes added to extend the existing `farm-api`
(.NET 8) and `farm-admin` (Angular 18) repositories, plus the new `farm-user`
scaffold, to deliver the full vision: Farm Management + Marketplace +
Investment Platform.

---

## Phase 1 - Farm Management (DONE - production-ready)

### farm-api

**New entities** (`Infrastructure/Farm.Domain/Entities/`):
- `Vaccine`, `VaccineSchedule` - vaccine catalog and scheduling per animal.
- `DiseaseRecord`, `Treatment` - illness tracking and medication courses.
- `FeedItem`, `FeedTransaction`, `FeedConsumption` - inventory and consumption.
- `GrowthLog` - per-animal weight/height history.
- `Alert` - system / business alerts surfaced to FE realtime.

**New enums** (`Infrastructure/Farm.Domain/Enum/`):
- `VaccineStatus`, `FeedTransactionType`, `AlertType`, `AlertSeverity`,
  `DiseaseSeverity`, `DiseaseStatus`.

**Repositories** (`Infrastructure/Farm.Domain/Repositories/`):
- VaccineRepository, VaccineScheduleRepository, DiseaseRecordRepository,
  TreatmentRepository, FeedItemRepository, FeedTransactionRepository,
  FeedConsumptionRepository, GrowthLogRepository, AlertRepository.

**Business services** (`Infrastructure/Farm.Business/Services/`):
- VaccineService, FeedService (with stock validation), GrowthLogService,
  DiseaseService, AlertService (auto SignalR push), ReportService.

**Hangfire jobs** (`Infrastructure/Farm.Business/Jobs/`):
- VaccineReminderJob - daily 06:00, raises alerts for due vaccines.
- WeightDropDetectorJob - daily 22:00, flags 5%+ weight loss in 7 days.
- StagnantGrowthJob - weekly, flags animals not gaining weight in 14 days.
- FeedLowStockJob - every 6 hours, alerts when stock <= threshold.
- DailyReportJob - 23:30, caches dashboard snapshot.

**Realtime** (`Services/Farm.Api/Hubs/`):
- `NotificationHub` (SignalR) - clients join `user:{id}` automatically and
  can opt-in to `farm:{farmId}` groups.
- `INotificationPublisher` (interface in Farm.Business) +
  `SignalRNotificationPublisher` (impl in Farm.Api).

**Controllers** (`Services/Farm.Api/Controllers/`):
- VaccineController, FeedController, GrowthLogController, DiseaseController,
  AlertController, ReportController, MediaController.

**Media storage**:
- `IMediaStorageService` + `LocalFileStorageService` (dev). Replace with
  `AzureBlobStorageService` in production.

**Wiring**:
- `Program.cs`: SignalR + Hangfire registered, recurring jobs scheduled,
  `/hubs/notifications` mapped, `/hangfire` dashboard exposed.
- `appsettings.json` connection string is reused for Hangfire (schema
  `Hangfire` is auto-created).

### farm-admin (Angular)

**New components** under `src/app/components/`:
- `vaccine-management/` - 2 tabs: catalog + upcoming schedule, "Đã tiêm" action.
- `feed-management/` - tabs: catalog, per-farm summary, low-stock alerts.
- `growth-log/` - pick animal, view history, add inline new entry.
- `alerts/` - inbox with realtime updates, mark read / mark all read.
- `reports/` - KPI dashboard + species/health distributions + 6-month trend.

**New services** under `src/app/services/`:
- `vaccine.service`, `feed.service`, `growth-log.service`, `disease.service`,
  `alert.service`, `report.service`, `realtime.service` (lazy-imports
  `@microsoft/signalr`).

**Models / enums** added under `src/app/models/`.

**Routes added** to `app-routing.module.ts`:
- `/app/vaccines`, `/app/feeds`, `/app/growth-logs`, `/app/alerts`,
  `/app/reports`.

**To enable realtime alerts** install:
```
npm install @microsoft/signalr
```
After login, call `RealtimeService.connect(() => oauthService.getAccessToken())`.

---

## Phase 2 - Marketplace (DONE - scaffolded)

### farm-api
- Entities: `Listing`, `ListingPhoto`, `ForumThread`, `ForumPost`, `Review`,
  `FarmVerification`, `ShippingPartner`.
- Service: `ListingService` with search by province / species / category.
- Controllers: `ListingController` (search public), `ForumController`,
  `MarketplaceController` (reviews, verifications, shipping partners).
- New enums: `ListingCategory`, `ListingStatus`, `FarmVerificationStatus`.

### farm-user (new repo - scaffold)
- `marketplace/marketplace-list.component` - browse listings with filter.
- `services/listing.service` - calls `/api/v1/listings`.
- `models/listing.model` - DTOs mirroring backend.
- `environments/environment.ts` - apiBaseUrl.
- See `farm-user/README.md` to bootstrap with `ng new`.

---

## Phase 3 - Investment Platform (DONE - scaffolded)

### farm-api
- Entities: `InvestmentOffer`, `InvestmentOrder`, `ShareCertificate`,
  `AnimalUpdate`, `CameraFeed`, `HarvestEvent`, `ProfitDistribution`.
- Service: `InvestmentService` with:
  - Concurrent-safe `PlaceOrder` (DB transaction).
  - `DistributeProfit` - revenue \* ProfitRatio split by share count.
- Controller: `InvestmentController` - offers, orders, harvest, distribution,
  animal updates.
- Enums: `InvestmentOfferStatus`, `InvestmentOrderStatus`, `HarvestType`,
  `AnimalUpdateType`.

### farm-user (scaffold)
- `investment/investment-list.component` - browse open offers, place order.
- `services/investment.service` - calls `/api/v1/investment`.

---

## Migration

Run a single EF Core migration to bring the database up to date:

```
dotnet ef migrations add Phase1to3 \
    -p Infrastructure/Farm.Domain/Farm.Domain.csproj \
    -s Services/Farm.Api/Farm.Api.csproj
```

The first run of `Farm.Api` will:
1. Apply EF migrations (auto in `Program.cs`).
2. Hangfire prepares the `Hangfire` schema.
3. Seed roles: `HO.SYSADMIN`, `FARM.OWNER`, `FARM.STAFF`, `VET`, `BUYER`,
   `INVESTOR`.

---

## Required NuGet packages added

In `Services/Farm.Api/Farm.Api.csproj`:
- `Hangfire.AspNetCore` 1.8.14
- `Hangfire.SqlServer` 1.8.14
- `Microsoft.AspNetCore.SignalR` 1.2.0

Run `dotnet restore` before the first build.

---

## Testing

A full QA test workbook with **81 cases** (Functional / Negative / Security /
Integration / Concurrency / Performance / UI) is generated at
`docs/02_test_cases.xlsx`. Each case maps to a phase (P1 / P2 / P3 / CC =
cross-cutting) and has a priority (High / Med / Low).

Critical High-priority cases to run first after each deploy:
- VAC-001..VAC-007 (vaccines + reminder job)
- FEED-001..FEED-006 (inventory + low-stock job)
- GRW-001, GRW-004 (growth + weight-drop job)
- ALT-001..ALT-005 (alerts + SignalR realtime)
- INV-003..INV-008, INV-011 (share purchase + profit distribution)
- SEC-001..SEC-004 (auth, CORS, XSS, SQLi)

---

## What is NOT included

These items are intentionally out of scope for this iteration but were
mentioned in the requirements:

- Real payment gateway for marketplace orders / investment orders. The
  current code records `TotalAmount` but does not call any PSP. Wire up
  VNPay / Momo / Stripe at the Order endpoints.
- Camera streaming server. The `CameraFeed` entity stores a URL only; bring
  your own RTSP / HLS / WebRTC origin (e.g. MediaMTX, Janus, Azure Media
  Services).
- Push notifications to mobile. Replace SignalR-only push with FCM / APNS
  if/when a mobile app is built.
- Advanced fraud / dispute handling on marketplace transactions.
- Wallet ledger - a real double-entry ledger should back ProfitDistribution
  payouts before going live.

---

## Files saved

- `docs/01_design_overview.docx` - design + ERD + API spec + role matrix.
- `docs/02_test_cases.xlsx` - 81 QA test cases with summary tab.
- `docs/00_CHANGELOG_v1.md` - this file.
