import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
@Injectable({
  providedIn: 'root'
})
export class HttpBaseService {

  constructor(private readonly http: HttpClient) { }

  public setHeaders() {
    let headers = new HttpHeaders();

    headers.append('Content-Type', 'application/json');
    headers.append('Accept', 'application/json');
    headers.append('Cache-Control', 'no-cache');
    // headers.append('Pragma', 'no-cache');
    return headers;
  }


  public async postDataAsync<T>(url: string, data: any) {
    const headers = this.setHeaders();
    const httpOptions = {
      headers: headers
    };

    return await this.http.post<T>(url, data, httpOptions).toPromise();
  }

  public async putDataAsync<T>(url: string, data: any) {
    const headers = this.setHeaders();
    const httpOptions = {
      headers: headers
    };

    return await this.http.put<T>(url, data, httpOptions).toPromise();
  }

  public async deleteDataAsync<T>(url: string) {
    const headers = this.setHeaders();
    const httpOptions = {
      headers: headers
    };

    return await this.http.delete<T>(url, httpOptions).toPromise();
  }

  public async uploadAsync<T>(url: string, upload: File) {
    // create multipart form for file
    let formData: FormData = new FormData();
    formData.append('upload', upload, upload.name);

    const headers = new HttpHeaders().append('Content-Disposition', 'mutipart/form-data');

    // POST
    return await this.http
      .post<T>(url, formData, { headers: headers }).toPromise();
  }

  
  public getData<T>(url: string): Observable<T> {
    return this.http.get<T>(url);
  }

  public async getDataAsync<T>(url: string) {
    return await this.http.get<T>(url).toPromise();
  }

  public upload(url: string, upload: File): Observable<object> {
    // create multipart form for file
    let formData: FormData = new FormData();
    formData.append('upload', upload, upload.name);

    const headers = new HttpHeaders().append('Content-Disposition', 'mutipart/form-data');

    // POST
    return this.http
      .post(url, formData, { headers: headers })
      .pipe(map(response => response));
  }

  downloadFile(url: string) {
    return this.http.get(url, { responseType: 'blob' });
  }
}
