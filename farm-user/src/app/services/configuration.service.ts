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
}

export interface IClientConfiguration {
  apiUrl: string;
  identityUrl: string;
}
