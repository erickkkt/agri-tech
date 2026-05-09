import { Injectable } from '@angular/core';
import { ApiEndPoints } from '../shared/config/api-end-points';
import { HttpBaseService } from '../shared/services/http-base.service';
import { Alert, AlertSummary } from '../models/alert.model';
import { PaginationResponse } from '../models/pagination-response.model';

@Injectable({ providedIn: 'root' })
export class AlertService {
  constructor(private api: ApiEndPoints, private http: HttpBaseService) { }

  getAlerts(pageIndex = 0, pageSize = 20, unread?: boolean, farmId?: string) {
    return this.http.getDataAsync<PaginationResponse<Alert>>(this.api.getAlertsPaging(pageIndex, pageSize, unread, farmId));
  }

  getSummary(farmId?: string) {
    return this.http.getDataAsync<AlertSummary>(this.api.getAlertSummary(farmId));
  }

  markRead(id: string) {
    return this.http.postDataAsync<Alert>(this.api.markAlertRead(id), {});
  }

  markAllRead(farmId?: string) {
    return this.http.postDataAsync<number>(this.api.markAllAlertsRead(farmId), {});
  }
}
