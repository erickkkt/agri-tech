# Farm User App (farm-user)

End-user Angular app for the Farm Management System.

## Audience
- **Buyers** - browse marketplace, place orders for breeding stock / antler / meat / eggs.
- **Investors** - browse open investment offers, buy shares, track animals via daily logs and camera feed, receive profit distributions.
- **General community** - read forum threads, leave reviews.

## Modules
- `marketplace/` - browse, search, listing detail (calls /api/v1/listings).
- `forum/` - threads + posts (calls /api/v1/forum).
- `investment/` - browse offers, buy shares, track investments (calls /api/v1/investment-offers, /api/v1/investment-orders, /api/v1/animals/{id}/updates).
- `wallet/` - balance + history (mock placeholder until real payments are wired).

## Status
This repository is currently a scaffold. The component skeletons live under
`src/app/<module>/`. To bootstrap a full Angular CLI project run:

```
ng new farm-user --routing --style=scss
```

then copy the existing component skeletons into the new project and install
the same dependencies as `farm-admin/package.json` (Angular 18 + Material).

## Recommended setup

```
npm install @angular/material @angular/cdk
npm install angular-oauth2-oidc
npm install @microsoft/signalr        # for realtime alerts
```

API base URL is configured via `environments/environment.ts` and points to
`farm-api` (default: https://localhost:44333).
