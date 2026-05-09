import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap, finalize } from 'rxjs/operators';
import { Router } from '@angular/router';
import { DialogService } from '../services/dialog.service';

@Injectable()
export class StandardHeaderInterceptor implements HttpInterceptor {
  private requests: HttpRequest<any>[] = [];
  constructor(public router: Router,
    private readonly _dialogService: DialogService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.requests.push(req);
    if (!req.headers.has('Content-Type') && !req.headers.has('Content-Disposition')) {
      req = req.clone({ headers: req.headers.set('Content-Type', 'application/json') });
    }

    req = req.clone({ headers: req.headers.set('Accept', 'application/json') });
    req = req.clone({ headers: req.headers.set('hostname', location.host) });

    return next.handle(req).pipe(
      tap(
        event => this.handleResponse(req, event),
        error => this.handleError(req, error),
        () => this.handleCompleteRequest(req)
      ),
      finalize(() => {
        this.removeRequest(req);
      })
    );
  }

  handleCompleteRequest(req: HttpRequest<any>) {
    this.removeRequest(req);
  }
  
  handleResponse(req: HttpRequest<any>, event: any) {
    if (event.type === 4 && event.status === 200) {
      // Handle successful response
    } else if (event.type === 4 && event.status !== 200) {
      this.handleError(req, event);
    }
  }

  handleError(req: HttpRequest<any>, event: any) {
    const contentType = event.headers.get('Content-Type');
    if (contentType === 'text/html') {
      console.error('MIME type error detected: Received text/html instead of JavaScript.');
      this.router.navigate([`/app-unavailable`]);
    }

    const error = event.error;
    if (event.status === 403 || event.status === 401) {
      //this._dialogService.openErrorDialog({ title: 'Access Denied', message: 'Sorry you do not have permission to this area' });
      this.router.navigate([`/app`]);
      setTimeout(() => {
        if (document && document.location && !window.location.href.includes('access-denied')) {
          document.location.reload();
        }
      }, 10);
    }
    else if (event.status === 409) {
      let conflictMessage = 'The conflict inserted data! Do you want to override it?';
      const primaryButtons = 'Close';

      this._dialogService.openErrorDialog({ title: 'Warning', message: error ? error : conflictMessage, primaryButtons: primaryButtons });
    }
    else if (event.status === 424) {
      let failedDependency = 'Dependencies data validation failed';
      const primaryButtons = 'Close';

      this._dialogService.openErrorDialog({ title: 'Warning', message: error ? error : failedDependency, primaryButtons: primaryButtons, dependenciesFailed: true });
    }
    else if (event.status === 500) {
      this.router.navigate([`/error`]);
    }
  }

  removeRequest(req: HttpRequest<any>) {
    const i = this.requests.findIndex(r => r.url === req.url);
    if (i >= 0) {
      this.requests.splice(i, 1);
    }
    const pending = this.requests.length > 0;
  }
}
