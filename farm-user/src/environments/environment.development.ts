// Dev env. `npm start` runs ng serve with proxy.conf.json that forwards /api/* → http://localhost:8080
// Keep apiUrl empty so ApiEndPoints produces relative URLs that go through the proxy.
export const environment = {
  production: false,
  test: false
};

export const configuration = {
  apiUrl: '',
  title: 'Farm User (dev)',
  version: 'v1'
};
