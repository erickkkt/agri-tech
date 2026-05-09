import { Injectable } from '@angular/core';
import { ApiEndPoints } from '../shared/config/api-end-points';
import { HttpBaseService } from '../shared/services/http-base.service';
import { Dashboard } from '../models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class ReportService {
  constructor(private api: ApiEndPoints, private http: HttpBaseService) { }

  getDashboard(farmId?: string) {
    return this.http.getDataAsync<Dashboard>(this.api.getDashboard(farmId));
  }
}
