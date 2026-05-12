import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

/**
 * Thin wrapper around HttpClient that keeps headers consistent and exposes
 * both Promise and Observable flavors. Mirrors farm-admin/HttpBaseService.
 */
@Injectable({ providedIn: 'root' })
export class HttpBaseService {

  constructor(private readonly http: HttpClient) { }

  private setHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'Cache-Control': 'no-cache'
    });
  }

  public getData<T>(url: string, params?: Record<string, any>): Observable<T> {
    return this.http.get<T>(url, { headers: this.setHeaders(), params: this.toParams(params) });
  }

  public async getDataAsync<T>(url: string, params?: Record<string, any>): Promise<T | undefined> {
    return await this.http
      .get<T>(url, { headers: this.setHeaders(), params: this.toParams(params) })
      .toPromise();
  }

  public async postDataAsync<T>(url: string, data: unknown): Promise<T | undefined> {
    return await this.http.post<T>(url, data, { headers: this.setHeaders() }).toPromise();
  }

  public async putDataAsync<T>(url: string, data: unknown): Promise<T | undefined> {
    return await this.http.put<T>(url, data, { headers: this.setHeaders() }).toPromise();
  }

  public async deleteDataAsync<T>(url: string): Promise<T | undefined> {
    return await this.http.delete<T>(url, { headers: this.setHeaders() }).toPromise();
  }

  private toParams(obj?: Record<string, any>): HttpParams | undefined {
    if (!obj) return undefined;
    let p = new HttpParams();
    for (const [k, v] of Object.entries(obj)) {
      if (v !== undefined && v !== null && v !== '') {
        p = p.set(k, String(v));
      }
    }
    return p;
  }
}
