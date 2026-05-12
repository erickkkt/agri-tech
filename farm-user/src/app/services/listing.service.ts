import { Injectable } from '@angular/core';
import { HttpBaseService } from '../shared/services/http-base.service';
import { ApiEndPoints } from '../shared/config/api-end-points';
import { Listing, ListingSearchQuery } from '../models/listing.model';

interface PaginationResponse<T> { items: T[]; total: number; }

@Injectable({ providedIn: 'root' })
export class ListingService {

  constructor(
    private readonly http: HttpBaseService,
    private readonly api: ApiEndPoints
  ) { }

  search(query: ListingSearchQuery): Promise<PaginationResponse<Listing> | undefined> {
    const params: Record<string, any> = {
      pageIndex: query.pageIndex ?? 0,
      pageSize: query.pageSize ?? 20
    };
    if (query.q) params['q'] = query.q;
    if (query.province) params['province'] = query.province;
    if (query.species !== undefined) params['species'] = query.species;
    if (query.category !== undefined) params['category'] = query.category;

    return this.http.getDataAsync<PaginationResponse<Listing>>(this.api.searchListings(), params);
  }

  getById(id: string): Promise<Listing | undefined> {
    return this.http.getDataAsync<Listing>(this.api.getListingById(id));
  }
}
