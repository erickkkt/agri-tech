import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { OAuthModuleConfig } from 'angular-oauth2-oidc';

@Injectable()
export class ConfigurationService {

  private configuration!: IServerConfiguration;
  private clientConfiguration!: IClientConfiguration | undefined;
  constructor(
    private moduleConfig: OAuthModuleConfig,
    private httpClient: HttpClient) { }

  loadRuntimeConfig() {
    return this.httpClient.get<IClientConfiguration>(`/assets/config.json`)
      .toPromise()
      .then(result => {
        if(result) {
          this.moduleConfig.resourceServer.allowedUrls!.push(result.apiUrl);
          this.moduleConfig.resourceServer.allowedUrls!.push(result.identityUrl);
          this.clientConfiguration = result;
        }
        return result;
      });
  }

  loadConfig(apiUrl: string) {
    return this.httpClient.get<IServerConfiguration>(apiUrl + '/api/v1/configuration/admin')
      .toPromise()
      .then(result => {
        console.dir(result);
        this.configuration = (result) as IServerConfiguration;
      }, error => console.error(error));
  }

  get apiBaseUrl() {
    return (this.configuration.apiBaseUrl);
  }

  get identityServerAddress() {
    return this.configuration.identityServerAddress;
  }

  get redirectUrl() {
    return this.configuration.redirectUrl;
  }

  get clientId() {
    return this.configuration.clientId;
  }

  get scope() {
    return this.configuration.scope;
  }

  get silentRefreshUrl() {
    return this.configuration.silentRefreshUrl;
  }

  get adminUrl() {
    return this.configuration.adminUrl;
  }

}

export interface IServerConfiguration {
  apiBaseUrl: string;
  clientId: string;
  identityServerAddress: string;
  redirectUrl: string;
  scope: string;
  silentRefreshUrl: string;
  adminUrl: string;
}

export interface IClientConfiguration {
  apiUrl: string;
  identityUrl: string;
}
