import { Injectable } from '@angular/core';
import { ApiEndPoints } from '../shared/config/api-end-points';
import { HttpBaseService } from '../shared/services/http-base.service';
import { CreateGrowthLog, GrowthLog } from '../models/growth-log.model';

@Injectable({ providedIn: 'root' })
export class GrowthLogService {
  constructor(private api: ApiEndPoints, private http: HttpBaseService) { }

  getByAnimal(animalId: string, take = 100) {
    return this.http.getDataAsync<GrowthLog[]>(this.api.getGrowthLogsByAnimal(animalId, take));
  }

  create(log: CreateGrowthLog) {
    return this.http.postDataAsync<string>(this.api.createGrowthLog(), log);
  }

  update(log: GrowthLog) {
    return this.http.putDataAsync<GrowthLog>(this.api.updateGrowthLog(), log);
  }

  delete(id: string) {
    return this.http.deleteDataAsync<void>(this.api.deleteGrowthLog(id));
  }
}
