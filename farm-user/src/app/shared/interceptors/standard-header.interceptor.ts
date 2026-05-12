import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

/**
 * Ensures Content-Type / Accept are set, and centralises HTTP error handling.
 * Mirrors farm-admin/StandardHeaderInterceptor but without the MatDialog dependency.
 */
@Injectable()
export class StandardHeaderInterceptor implements HttpInterceptor {

  constructor(private readonly router: Router) { }

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
    if (err.status === 401 || err.status === 403) {
      // angular-oauth2-oidc will already have triggered a re-login if the token is bad;
      // for explicit anonymous calls, just bounce back to home.
      this.router.navigateByUrl('/');
    } else if (err.status >= 500) {
      console.error('Server error:', err);
    }
    return throwError(() => err);
  }
}
