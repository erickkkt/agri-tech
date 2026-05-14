import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { AuthService } from '../../services/auth.service';

/**
 * Attaches `Authorization: Bearer <token>` to outgoing requests when the user is logged in.
 *
 * Skips:
 *  - non-API URLs (config.json, static assets, etc.) — we only care about our own backend.
 *  - the auth endpoints themselves (login / register accept anonymous bodies).
 */
@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private readonly auth: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.auth.accessToken;
    if (!token) return next.handle(req);

    if (req.url.includes('/auth/login') || req.url.includes('/auth/register')) {
      return next.handle(req);
    }

    const authed = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`)
    });
    return next.handle(authed);
  }
}
