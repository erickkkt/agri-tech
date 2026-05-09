import { Injectable } from '@angular/core';
import { ApiEndPoints } from '../shared/config/api-end-points';
import { HttpBaseService } from '../shared/services/http-base.service';
import { PaginationResponse } from '../models/pagination-response.model';
import { CreateFeedTransaction, FeedConsumption, FeedItem, FeedStockLevel, FeedSummary, FeedTransaction } from '../models/feed.model';

@Injectable({ providedIn: 'root' })
export class FeedService {
  constructor(private api: ApiEndPoints, private http: HttpBaseService) { }

  getItems(pageIndex = 0, pageSize = 10) {
    return this.http.getDataAsync<PaginationResponse<FeedItem>>(this.api.getFeedItemsPaging(pageIndex, pageSize));
  }

  createItem(item: FeedItem) {
    return this.http.postDataAsync<string>(this.api.createFeedItem(), item);
  }

  updateItem(item: FeedItem) {
    return this.http.putDataAsync<FeedItem>(this.api.updateFeedItem(), item);
  }

  createTransaction(tx: CreateFeedTransaction) {
    return this.http.postDataAsync<string>(this.api.createFeedTransaction(), tx);
  }

  getTransactions(filters: { farmId?: string; feedItemId?: string; from?: string; to?: string } = {}) {
    let url = this.api.getFeedTransactions();
    const qs: string[] = [];
    if (filters.farmId) qs.push(`farmId=${filters.farmId}`);
    if (filters.feedItemId) qs.push(`feedItemId=${filters.feedItemId}`);
    if (filters.from) qs.push(`from=${filters.from}`);
    if (filters.to) qs.push(`to=${filters.to}`);
    if (qs.length) url += `?${qs.join('&')}`;
    return this.http.getDataAsync<FeedTransaction[]>(url);
  }

  getSummary(farmId: string) {
    return this.http.getDataAsync<FeedSummary>(this.api.getFeedSummary(farmId));
  }

  getLowStock() {
    return this.http.getDataAsync<FeedStockLevel[]>(this.api.getFeedLowStock());
  }

  createConsumption(c: FeedConsumption) {
    return this.http.postDataAsync<string>(this.api.createFeedConsumption(), c);
  }
}
