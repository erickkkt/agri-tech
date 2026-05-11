// Production env. In docker, nginx in farm-user reverse-proxies "/api/" → http://farm-api:8080/
// so apiBaseUrl is empty (relative same-origin), which sidesteps CORS entirely.
export const environment = {
  production: true,
  apiBaseUrl: '',
  apiVersion: 'v1'
};
