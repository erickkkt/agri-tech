import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, firstValueFrom } from 'rxjs';

import { ConfigurationService } from './configuration.service';

/**
 * End-user auth backed by /api/v1/auth/{login,register} on Farm.Api.
 *
 * - JWT is stored in localStorage so login survives a browser restart.
 * - `isAuthenticated$` and `currentUser$` are reactive — the header / guards
 *   subscribe to them instead of polling.
 * - The HTTP interceptor (AuthInterceptor) reads the token via `accessToken`
 *   and attaches `Authorization: Bearer <token>` to API calls.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {

  private static readonly TOKEN_KEY = 'agritech.userToken';
  private static readonly USER_KEY = 'agritech.user';

  private readonly _isAuthenticated$ = new BehaviorSubject<boolean>(false);
  private readonly _currentUser$ = new BehaviorSubject<AuthUser | null>(null);

  /** Emits true while a valid token is held. */
  readonly isAuthenticated$ = this._isAuthenticated$.asObservable();
  /** Emits the current user payload (id, email, displayName) or null. */
  readonly currentUser$ = this._currentUser$.asObservable();

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router,
    private readonly config: ConfigurationService
  ) {
    this.restoreFromStorage();
  }

  // -------- Public API --------

  get accessToken(): string | null {
    return localStorage.getItem(AuthService.TOKEN_KEY);
  }

  hasValidToken(): boolean {
    const token = this.accessToken;
    if (!token) return false;
    const exp = this.tokenExpiryEpochSec(token);
    if (!exp) return false;
    return Date.now() / 1000 < exp - 30; // 30s safety margin
  }

  async login(email: string, password: string): Promise<void> {
    const res = await firstValueFrom(this.http.post<AuthResponse>(
      `${this.apiBase}/auth/login`, { email, password }
    ));
    this.applyAuthResponse(res);
  }

  async register(payload: RegisterPayload): Promise<void> {
    const res = await firstValueFrom(this.http.post<AuthResponse>(
      `${this.apiBase}/auth/register`, payload
    ));
    this.applyAuthResponse(res);
  }

  logout(redirectTo: string = '/'): void {
    localStorage.removeItem(AuthService.TOKEN_KEY);
    localStorage.removeItem(AuthService.USER_KEY);
    this._isAuthenticated$.next(false);
    this._currentUser$.next(null);
    this.router.navigateByUrl(redirectTo);
  }

  /**
   * Called by the auth guard when an unauthenticated user hits a protected route.
   * Sends them to /login with the original URL preserved so we can bounce back.
   */
  redirectToLogin(returnUrl?: string): void {
    this.router.navigate(['/login'], { queryParams: returnUrl ? { returnUrl } : undefined });
  }

  // -------- Internals --------

  private get apiBase(): string {
    return `${this.config.apiBaseUrl}/api/v1`;
  }

  private applyAuthResponse(res: AuthResponse): void {
    localStorage.setItem(AuthService.TOKEN_KEY, res.accessToken);
    localStorage.setItem(AuthService.USER_KEY, JSON.stringify(res.user));
    this._isAuthenticated$.next(true);
    this._currentUser$.next(res.user);
  }

  private restoreFromStorage(): void {
    if (!this.hasValidToken()) {
      // Expired or missing — clean up silently.
      localStorage.removeItem(AuthService.TOKEN_KEY);
      localStorage.removeItem(AuthService.USER_KEY);
      return;
    }
    const userJson = localStorage.getItem(AuthService.USER_KEY);
    if (userJson) {
      try {
        this._currentUser$.next(JSON.parse(userJson));
      } catch { /* corrupted — ignore */ }
    }
    this._isAuthenticated$.next(true);
  }

  private tokenExpiryEpochSec(token: string): number | null {
    // JWT: header.payload.signature — payload is base64url JSON with `exp` claim.
    try {
      const payload = token.split('.')[1];
      const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
      const parsed = JSON.parse(json);
      return typeof parsed.exp === 'number' ? parsed.exp : null;
    } catch {
      return null;
    }
  }
}

// -------- DTOs --------

export interface AuthUser {
  id: string;
  email: string;
  displayName: string;
}

export interface AuthResponse {
  accessToken: string;
  expiresAt: string; // ISO
  user: AuthUser;
}

export interface RegisterPayload {
  email: string;
  password: string;
  displayName?: string;
  phoneNumber?: string;
}
