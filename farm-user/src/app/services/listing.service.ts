import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Listing, ListingSearchQuery } from '../models/listing.model';
import { environment } from '../../environments/environment';

interface PaginationResponse<T> { items: T[]; total: number; }

@Injectable({ providedIn: 'root' })
export class ListingService {
  private base = `${environment.apiBaseUrl}/api/v1/listings`;

  constructor(private http: HttpClient) { }

  search(query: ListingSearchQuery) {
    const params: any = {};
    if (query.q) params.q = query.q;
    if (query.province) params.province = query.province;
    if (query.species !== undefined) params.species = query.species;
    if (query.category !== undefined) params.category = query.category;
    params.pageIndex = query.pageIndex ?? 0;
    params.pageSize = query.pageSize ?? 20;
    return this.http.get<PaginationResponse<Listing>>(this.base, { params }).toPromise();
  }

  getById(id: string) {
    return this.http.get<Listing>(`${this.base}/${id}`).toPromise();
  }
}
