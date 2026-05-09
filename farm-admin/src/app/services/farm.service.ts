import { Injectable } from '@angular/core';
import { ApiEndPoints } from '../shared/config/api-end-points';
import { PagingConstant } from '../shared/constants/constants';
import { HttpBaseService } from '../shared/services/http-base.service';
import { Farm } from '../models/farm.model';
import { PaginationResponse } from '../models/pagination-response.model';

@Injectable({
  providedIn: "root",
})
export class FarmService {
  constructor(
    private api: ApiEndPoints,
    private readonly _httpService: HttpBaseService
  ) { }

  async getFarms(sortField: string, sortDirection = "asc", pageIndex = PagingConstant.pageIndex, pageSize = PagingConstant.pageSize): Promise<PaginationResponse<Farm> | undefined> {
    return await this._httpService.getDataAsync<PaginationResponse<Farm>>(this.api.getFarmsWithPaging(sortField, sortDirection, pageIndex, pageSize));
  }

  async getActiveFarms(): Promise<Farm[] | undefined> {
    return await this._httpService.getDataAsync<Farm[]>(this.api.getActiveFarms());
  }

  async getFarmById(farmId: string) : Promise<Farm | undefined> {
    return await this._httpService.getDataAsync<Farm>(this.api.getFarmByFarmId(farmId));
  }

  async createFarm(isCreatingDefaultBranch: boolean, farm: Farm) : Promise<Farm | undefined> {
    return await this._httpService.postDataAsync<Farm>(this.api.createFarm(), {
      isCreatingDefaultBranch,
      farm,
    });
  }

  async updateFarm(data: Farm): Promise<Farm | undefined> {
    return await this._httpService.putDataAsync<Farm>(
      this.api.updateFarm(),
      data
    );
  }
}
