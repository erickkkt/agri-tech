# Farm User App (farm-user)

End-user Angular 19 SPA for the Agri-Tech Farm Management System.

## Audience
- **Buyers** — browse marketplace, place orders for breeding stock, antler, meat, eggs.
- **Investors** — browse open investment offers, buy shares, track animals.
- **Community** — read forum threads, leave reviews.

## Modules
- `home/` — landing page.
- `marketplace/` — browse + filter listings (`GET /api/v1/listings`).
- `investment/` — browse offers, place orders (`/api/v1/investment/...`).
- `forum/` — list threads (`GET /api/v1/forum/threads`).

## Run with Docker (recommended)

From the monorepo root:

```bash
docker compose up --build
```

Then open:
- http://localhost:4300 — farm-user
- http://localhost:4200 — farm-admin
- http://localhost:8080/swagger — farm-api

The nginx in this container reverse-proxies `/api/*` to `farm-api:8080`, so
the browser always sees same-origin requests (no CORS configuration needed).

## Local dev (without Docker)

```bash
npm install
npm start         # ng serve on port 4300 with /api proxy → localhost:8080
```

`proxy.conf.json` proxies `/api/*` to a locally-running farm-api on port 8080.

## Tech stack
- Angular 19 (module-based, NOT standalone)
- HttpClient via `provideHttpClient(withInterceptorsFromDi())`
- No Material — plain CSS to keep the bundle small
- nginx 1.26 (alpine) as runtime web server
