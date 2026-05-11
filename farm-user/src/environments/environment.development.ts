// Dev env. `npm start` runs ng serve with proxy.conf.json that forwards /api/* → http://localhost:8080
// Keep apiBaseUrl empty to use the proxy. Override here if you want to hit the API directly.
export const environment = {
  production: false,
  apiBaseUrl: '',
  apiVersion: 'v1'
};
