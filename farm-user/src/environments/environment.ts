// Production env. Real apiUrl is loaded at runtime from /assets/config.json (token-replaced
// during CI/CD), so apiUrl below is just a fallback for dev.
export const environment = {
  production: true,
  test: false
};

export const configuration = {
  apiUrl: '',
  title: 'Farm User',
  version: 'v1'
};
