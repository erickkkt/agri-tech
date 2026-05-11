# =====================================================================
# farm-user - Angular 19 SPA served by nginx
# nginx also reverse-proxies /api/ → farm-api:8080 so the browser is
# same-origin (no CORS pain).
# =====================================================================

# ---------- build ----------
FROM node:20.11.1-alpine AS build
WORKDIR /app

# Install deps first for layer caching
COPY package.json package-lock.json* ./
RUN npm install --legacy-peer-deps

# Copy source and build
COPY . .
ENV PATH=/app/node_modules/.bin:$PATH
RUN ng build --configuration=production --output-path=dist

# ---------- runtime ----------
FROM nginx:1.26-alpine

# Angular 17+ outputs into dist/<project>/browser
COPY --from=build /app/dist/browser /usr/share/nginx/html

# SPA-friendly nginx config with /api/ reverse proxy to farm-api
COPY docker/nginx-conf/default.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
