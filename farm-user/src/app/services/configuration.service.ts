import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

/**
 * Loads runtime config from /assets/config.json (token-replaced during CI/CD).
 * The config.json is the single source of truth for API base URL.
 *
 * NOTE: This used to also configure angular-oauth2-oidc allowedUrls — we removed
 * the OIDC dependency when end-user auth moved to a local JWT issued by our own
 * /api/v1/auth/* endpoints.
 */
@Injectable()
export class ConfigurationService {

  private clientConfig?: IClientConfiguration;

  constructor(private httpClient: HttpClient) { }

  /**
   * Called via APP_INITIALIZER. Loads config.json once.
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
          result.apiUrl = (result.apiUrl ?? '').replace(/\/+$/, '');
          this.clientConfig = result;
        }
        return result;
      })
      .catch(err => {
        console.error('Failed to load /assets/config.json', err);
        this.clientConfig = { apiUrl: '' };
        return this.clientConfig;
      });
  }

  get apiBaseUrl(): string {
    return this.clientConfig?.apiUrl ?? '';
  }
}

export interface IClientConfiguration {
  apiUrl: string;
  /** Kept for backwards-compat with existing config.json deployments; ignored. */
  identityUrl?: string;
}
