import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, Route, UrlSegment, CanActivateChild, CanLoad, Router } from '@angular/router';
import { Observable, combineLatest } from 'rxjs';
import { filter, map, tap, take } from 'rxjs/operators';

import { AuthService } from './auth.service';
import { DialogService } from '../shared/services/dialog.service';

@Injectable()
export class AuthGuardService implements CanActivate, CanActivateChild, CanLoad {
  private isAuthenticated: boolean | undefined;

  constructor(
    private authService: AuthService,
    private readonly dialogService: DialogService
  ) {
    this.authService.isAuthenticated$.subscribe(i => this.isAuthenticated = i);
  }

  isAuthenticatedSystem(state: RouterStateSnapshot): Observable<boolean> {
    return this.authService.isDoneLoading$
      .pipe(filter(isDone => isDone === true))
      .pipe(tap(_ => this.isAuthenticated || this.authService.login(state.url)))
      .pipe(map(_ => this.isAuthenticated === true));
  }

  private isAuthorized(next: ActivatedRouteSnapshot) {
    return combineLatest([this.authService.loadedUserInfo$, this.authService.userInfo$])
      .pipe(
        map(([isloaded, user]) => {
          // The empty-path child (e.g. /app home) is always allowed once authenticated.
          if (next.routeConfig && next.routeConfig.path === "") return true;

          // No required roles on the route → any authenticated user passes.
          const requiredRoles: string[] | undefined = next.data ? next.data['roles'] : undefined;
          if (!requiredRoles || requiredRoles.length === 0) return true;

          // User profile not loaded yet (transient on first nav) → allow optimistically;
          // canActivate will be re-run by the router on subsequent navigations.
          if (!isloaded) return true;

          // User profile loaded but no `role` field returned by /users/info
          // (common when the AD tenant doesn't expose role claims) → authentication alone is enough.
          if (!user || !user['role']) return true;

          // Strict role check — only when server returned a concrete role.
          return requiredRoles.indexOf(user['role']) !== -1;
        }), take(1));
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot,
  ): Observable<boolean> {
    return combineLatest(
      [this.isAuthenticatedSystem(state), this.isAuthorized(route)]
    ).pipe(
      tap(([isAuthenticated, isAuthorized]) => {
        if (isAuthenticated) {
          if (!isAuthorized) {
            this.dialogService.openErrorDialog({ 'title': 'Access Denied', 'message': 'Sorry you do not have permission to this area' });
          }
        }
      }),
      map(([isAuthenticated, isAuthorized]) => isAuthenticated && isAuthorized),
    );
  }

  canLoad(route: Route, segments: UrlSegment[]): Observable<boolean> | Promise<boolean> | boolean {
    return this.authService.isDoneLoading$
      .pipe(filter(isDone => isDone))
      .pipe(tap(_ => this.isAuthenticated))
      .pipe(map(_ => this.isAuthenticated === true));
  }

  canActivateChild(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return combineLatest(
      [this.isAuthenticatedSystem(state),
      this.isAuthorized(next)]
    ).pipe(
      tap(([isAuthenticated, isAuthorized]) => {
        if (isAuthenticated) {
          if (!isAuthorized) {
            this.dialogService.openErrorDialog({ 'title': 'Access Denied', 'message': 'Sorry you do not have permission to this area' });
          }
        }
      }),
      map(([isAuthenticated, isAuthorized]) => isAuthenticated && isAuthorized),
    );
  }
}
