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
          if (next.routeConfig && next.routeConfig.path === "") {
            return true;
          }
          else {
            if (isloaded) {
              return (user && next.data['roles'] && next.data['roles'].indexOf(user['role']) !== -1)
            }
            else { return true; }
          }
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
