import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { AuthService } from '../../services/auth.service';

/**
 * Ensures Content-Type / Accept are set, and centralises HTTP error handling.
 * 401 → wipe local token + bounce to /login (with returnUrl). 5xx → log.
 */
@Injectable()
export class StandardHeaderInterceptor implements HttpInterceptor {

  constructor(
    private readonly router: Router,
    private readonly auth: AuthService
  ) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let modified = req;

    if (!modified.headers.has('Content-Type') && !modified.headers.has('Content-Disposition')) {
      modified = modified.clone({ headers: modified.headers.set('Content-Type', 'application/json') });
    }
    modified = modified.clone({ headers: modified.headers.set('Accept', 'application/json') });

    return next.handle(modified).pipe(
      catchError((err: HttpErrorResponse) => this.handleError(err))
    );
  }

  private handleError(err: HttpErrorResponse): Observable<never> {
    if (err.status === 401) {
      // Token expired / invalid — purge local state and redirect to login.
      // Skip auto-redirect when the failing call IS /auth/login: that means wrong
      // credentials, the login form will show the error itself.
      const url = err.url ?? '';
      if (!url.includes('/auth/login') && !url.includes('/auth/register')) {
        this.auth.logout(this.router.url.startsWith('/login') ? '/' : '/login');
      }
    } else if (err.status >= 500) {
      console.error('Server error:', err);
    }
    return throwError(() => err);
  }
}
