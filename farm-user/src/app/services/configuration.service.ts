import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { OAuthModuleConfig } from 'angular-oauth2-oidc';

/**
 * Loads runtime config from /assets/config.json (token-replaced during CI/CD).
 * Mirrors farm-admin/ConfigurationService but simpler — we don't call a backend
 * /configuration endpoint, the config.json is the single source of truth.
 */
@Injectable()
export class ConfigurationService {

  private clientConfig?: IClientConfiguration;

  constructor(
    private moduleConfig: OAuthModuleConfig,
    private httpClient: HttpClient
  ) { }

  /**
   * Called via APP_INITIALIZER. Loads config.json once and pushes apiUrl/identityUrl
   * into the OAuth allow-list so angular-oauth2-oidc attaches the Bearer token.
   */
  loadRuntimeConfig(): Promise<IClientConfiguration | undefined> {
    return this.httpClient.get<IClientConfiguration>(`/assets/config.json`)
      .toPromise()
      .then(result => {
        if (result) {
          // If the placeholder wasn't replaced during CI/CD, fall back to same-origin
          // so the app still boots (useful for `npm start` + nginx proxy).
          if (!result.apiUrl || result.apiUrl === 'API_URL') {
            result.apiUrl = '';
          }
          if (!result.identityUrl || result.identityUrl === 'AUTH_URL') {
            result.identityUrl = '';
          }

          // Defensive: strip common wrong suffixes so `<issuer>/.well-known/openid-configuration`
          // resolves correctly even if CI/CD injects the authorize endpoint by mistake.
          // Valid issuers look like:
          //   https://login.microsoftonline.com/<tenant>/v2.0
          //   https://<tenant>.b2clogin.com/<tenant>.onmicrosoft.com/<policy>/v2.0
          // NOT .../oauth2/v2.0/authorize
          result.identityUrl = this.normalizeIssuer(result.identityUrl);
          result.apiUrl = (result.apiUrl ?? '').replace(/\/+$/, '');

          if (result.apiUrl) this.moduleConfig.resourceServer.allowedUrls!.push(result.apiUrl);
          if (result.identityUrl) this.moduleConfig.resourceServer.allowedUrls!.push(result.identityUrl);

          this.clientConfig = result;
        }
        return result;
      })
      .catch(err => {
        console.error('Failed to load /assets/config.json', err);
        this.clientConfig = { apiUrl: '', identityUrl: '' };
        return this.clientConfig;
      });
  }

  get apiBaseUrl(): string {
    return this.clientConfig?.apiUrl ?? '';
  }

  get identityServerAddress(): string {
    return this.clientConfig?.identityUrl ?? '';
  }

  /**
   * Strip authorize/token endpoint suffixes from an issuer URL so the OIDC discovery
   * URL `<issuer>/.well-known/openid-configuration` resolves correctly.
   *
   * Wrong (authorize endpoint)   → corrected (issuer base)
   *   .../<tenant>/oauth2/v2.0/authorize → .../<tenant>/v2.0
   *   .../<tenant>/oauth2/authorize      → .../<tenant>
   *   .../<tenant>/v2.0/                  → .../<tenant>/v2.0
   */
  private normalizeIssuer(url: string): string {
    if (!url) return '';
    return url
      .replace(/\/oauth2\/v2\.0\/(authorize|token)\/?$/i, '/v2.0')
      .replace(/\/oauth2\/(authorize|token)\/?$/i, '')
      .replace(/\/authorize\/?$/i, '')
      .replace(/\/+$/, '');
  }
}

export interface IClientConfiguration {
  apiUrl: string;
  identityUrl: string;
}
