import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';

import { AuthService } from './auth.service';

/**
 * Use on routes that require a logged-in end-user. Anonymous browsing of the
 * listing GRID is fine; this guard only protects detail / commitment routes.
 *
 * On failure: redirect to /login?returnUrl=<original>, preserving the deep link
 * so the user comes back to where they were after authenticating.
 */
@Injectable({ providedIn: 'root' })
export class AuthGuardService implements CanActivate {

  constructor(
    private readonly auth: AuthService,
    private readonly router: Router
  ) { }

  canActivate(_: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    if (this.auth.hasValidToken()) return true;
    return this.router.createUrlTree(['/login'], {
      queryParams: { returnUrl: state.url }
    });
  }
}
