import { Injectable } from '@angular/core';
import { CanActivate, CanActivateChild, CanLoad, Router, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { filter, map, take, tap } from 'rxjs/operators';

import { AuthService } from './auth.service';

/**
 * Use this guard on routes that require a logged-in user (e.g. /investments/checkout,
 * /forum/create). Marketplace browsing is intentionally NOT guarded.
 *
 * Simpler than farm-admin's guard because farm-user has no role-based UI yet.
 */
@Injectable({ providedIn: 'root' })
export class AuthGuardService implements CanActivate, CanActivateChild, CanLoad {

  private isAuthenticated = false;

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {
    this.authService.isAuthenticated$.subscribe(v => this.isAuthenticated = v);
  }

  canActivate(_: unknown, state: RouterStateSnapshot): Observable<boolean> {
    return this.checkAuth(state.url);
  }

  canActivateChild(_: unknown, state: RouterStateSnapshot): Observable<boolean> {
    return this.checkAuth(state.url);
  }

  canLoad(): Observable<boolean> {
    return this.authService.isDoneLoading$
      .pipe(filter(done => done))
      .pipe(take(1))
      .pipe(map(_ => this.isAuthenticated));
  }

  private checkAuth(targetUrl: string): Observable<boolean> {
    return this.authService.isDoneLoading$
      .pipe(filter(done => done))
      .pipe(take(1))
      .pipe(tap(_ => {
        if (!this.isAuthenticated) {
          this.authService.login(targetUrl);
        }
      }))
      .pipe(map(_ => this.isAuthenticated));
  }
}
