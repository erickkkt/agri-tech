import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwksValidationHandler } from 'angular-oauth2-oidc-jwks';
import { AuthConfig, OAuthErrorEvent, OAuthService, NullValidationHandler, UserInfo } from 'angular-oauth2-oidc';
import { BehaviorSubject, combineLatest, Observable, ReplaySubject } from 'rxjs';
import { filter, map } from 'rxjs/operators';

import { HttpBaseService } from '../shared/services/http-base.service';
import { ApiEndPoints } from '../shared/config/api-end-points';
import { ConfigurationService } from './configuration.service';

@Injectable({ providedIn: 'root' })

export class AuthService {

  private isAuthenticatedSubject$ = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject$.asObservable();

  private isDoneLoadingSubject$ = new ReplaySubject<boolean>();
  public isDoneLoading$ = this.isDoneLoadingSubject$.asObservable();

  private loadedUserInfoSubject$ = new BehaviorSubject<boolean>(false);
  public loadedUserInfo$ = this.loadedUserInfoSubject$.asObservable();


  userInfo$: BehaviorSubject<UserInfo | null> = new BehaviorSubject<UserInfo | null>(null);

  private navigateToLoginPage() {
    this.router.navigateByUrl('/should-login');
  }

  constructor(
    private readonly http: HttpBaseService,
    private readonly api: ApiEndPoints,
    private readonly configurationService: ConfigurationService,
    private readonly oauthService: OAuthService,
    private readonly router: Router) {

    this.oauthService.events
      .subscribe(_ => {
        this.isAuthenticatedSubject$.next(this.oauthService.hasValidAccessToken());
      });

    this.oauthService.events
      .pipe(filter(e => ['session_terminated', 'session_error'].includes(e.type)))
      .subscribe(e => this.navigateToLoginPage());

    const authConfig: AuthConfig = {
      clientId: this.configurationService.clientId,
      issuer: this.configurationService.identityServerAddress,
      redirectUri: this.configurationService.redirectUrl,
      responseType: 'token id_token',
      scope: this.configurationService.scope,
      silentRefreshRedirectUri: this.configurationService.silentRefreshUrl,
      timeoutFactor: 0.8,
      requestAccessToken: true,
      skipIssuerCheck: true,
      clearHashAfterLogin: true,
      oidc: true,
      strictDiscoveryDocumentValidation: false,
    };

    this.oauthService.configure(authConfig);
    this.oauthService.tokenValidationHandler = new NullValidationHandler();
  }

  public runInitialLoginSequence(): Promise<void> {
    if (location.hash) {
      console.log('Encountered hash fragment, plotting as table...');
      console.table(location.hash.substr(1).split('&').map(kvp => kvp.split('=')));
    }

    return this.oauthService.loadDiscoveryDocument(this.configurationService.identityServerAddress + '/.well-known/openid-configuration')
      .then(() => {
        this.oauthService.tryLoginImplicitFlow().then(() => {
          if (this.oauthService.hasValidAccessToken()) {
            this.getUerProfile();
            return Promise.resolve();
          }
          return Promise.resolve(); // Ensure all code paths return a value
        })
          .then(() => {
            this.isDoneLoadingSubject$.next(true);
            if (this.oauthService.state && this.oauthService.state !== 'undefined' && this.oauthService.state !== 'null') {
              console.log('There was state, so we are sending you to: ' + this.oauthService.state);
              this.router.navigateByUrl('/app');
            }
          })
          .catch(() => {
            this.isDoneLoadingSubject$.next(true)
          });
      })
  }

  public login(targetUrl?: string) {
    this.oauthService.initImplicitFlow(encodeURIComponent(targetUrl || this.router.url));
  }

  public logout() { this.oauthService.logOut(); }
  public refresh() { this.oauthService.silentRefresh(); }
  public hasValidToken() { return this.oauthService.hasValidAccessToken(); }

  public get accessToken() { return this.oauthService.getAccessToken(); }

  public get identityClaims() { return this.oauthService.getIdentityClaims(); }

  public get idToken() { return this.oauthService.getIdToken(); }

  public get logoutUrl() { return this.oauthService.logoutUrl; }

  private getUerProfile() {
    this.http.getDataAsync<UserInfo>(this.api.getUserInfo()).then((user) => {
      return this.userInfo$.next(user ?? null);
    })
      .finally(() => this.loadedUserInfoSubject$.next(true))
  }
}
