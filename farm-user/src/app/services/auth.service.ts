import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AuthConfig, NullValidationHandler, OAuthService, UserInfo } from 'angular-oauth2-oidc';
import { BehaviorSubject, ReplaySubject } from 'rxjs';
import { filter } from 'rxjs/operators';

import { HttpBaseService } from '../shared/services/http-base.service';
import { ApiEndPoints } from '../shared/config/api-end-points';
import { ConfigurationService } from './configuration.service';

/**
 * End-user auth. Same Azure AD B2C / implicit flow shape as farm-admin so a user
 * can be logged in across both apps if they share a tenant. Browsing the
 * marketplace + forum + investment listings does NOT require auth; auth is
 * only enforced when placing an order / posting / etc. (see AuthGuardService).
 */
@Injectable({ providedIn: 'root' })
export class AuthService {

  private isAuthenticatedSubject$ = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject$.asObservable();

  private isDoneLoadingSubject$ = new ReplaySubject<boolean>(1);
  public isDoneLoading$ = this.isDoneLoadingSubject$.asObservable();

  private loadedUserInfoSubject$ = new BehaviorSubject<boolean>(false);
  public loadedUserInfo$ = this.loadedUserInfoSubject$.asObservable();

  public userInfo$ = new BehaviorSubject<UserInfo | null>(null);

  constructor(
    private readonly http: HttpBaseService,
    private readonly api: ApiEndPoints,
    private readonly configurationService: ConfigurationService,
    private readonly oauthService: OAuthService,
    private readonly router: Router
  ) {
    // Track token validity from the oauth event stream
    this.oauthService.events.subscribe(_ => {
      this.isAuthenticatedSubject$.next(this.oauthService.hasValidAccessToken());
    });

    this.oauthService.events
      .pipe(filter(e => ['session_terminated', 'session_error'].includes(e.type)))
      .subscribe(_ => this.navigateToLoginPage());

    // We can't configure OAuth until ConfigurationService has loaded config.json,
    // which happens in APP_INITIALIZER before this service is constructed. So
    // by the time we get here, identityServerAddress is already populated.
    const authConfig: AuthConfig = {
      issuer: this.configurationService.identityServerAddress || undefined,
      redirectUri: window.location.origin,
      postLogoutRedirectUri: window.location.origin,
      silentRefreshRedirectUri: window.location.origin + '/assets/silent-refresh.html',
      responseType: 'token id_token',
      scope: 'openid profile',
      timeoutFactor: 0.8,
      requestAccessToken: true,
      skipIssuerCheck: true,
      clearHashAfterLogin: true,
      oidc: true,
      strictDiscoveryDocumentValidation: false
    };

    this.oauthService.configure(authConfig);
    this.oauthService.tokenValidationHandler = new NullValidationHandler();
  }

  /**
   * Run once on app bootstrap. Loads the OIDC discovery doc, tries implicit
   * login (parsing any token in the URL hash), and pulls user profile.
   */
  public async runInitialLoginSequence(): Promise<void> {
    if (!this.configurationService.identityServerAddress) {
      // No identity URL configured -- treat as anonymous app, mark loading done.
      this.isDoneLoadingSubject$.next(true);
      return;
    }

    try {
      await this.oauthService.loadDiscoveryDocument(
        this.configurationService.identityServerAddress + '/.well-known/openid-configuration'
      );
      await this.oauthService.tryLoginImplicitFlow();
      if (this.oauthService.hasValidAccessToken()) {
        this.getUserProfile();
      }
    } catch (e) {
      console.error('OIDC initial login failed', e);
    } finally {
      this.isDoneLoadingSubject$.next(true);
    }
  }

  public login(targetUrl?: string): void {
    this.oauthService.initImplicitFlow(encodeURIComponent(targetUrl || this.router.url));
  }

  public logout(): void {
    this.oauthService.logOut();
  }

  public refresh(): void {
    this.oauthService.silentRefresh().catch(err => console.error('silentRefresh failed', err));
  }

  public hasValidToken(): boolean {
    return this.oauthService.hasValidAccessToken();
  }

  public get accessToken(): string { return this.oauthService.getAccessToken(); }
  public get identityClaims(): object | null { return this.oauthService.getIdentityClaims(); }
  public get idToken(): string { return this.oauthService.getIdToken(); }

  private navigateToLoginPage(): void {
    // For end-user app, just send them back home rather than a dedicated should-login page.
    this.router.navigateByUrl('/');
  }

  private getUserProfile(): void {
    this.http.getDataAsync<UserInfo>(this.api.getUserInfo())
      .then(user => this.userInfo$.next(user ?? null))
      .catch(err => console.error('Failed to load user profile', err))
      .finally(() => this.loadedUserInfoSubject$.next(true));
  }
}
