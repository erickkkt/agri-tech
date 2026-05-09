import { Injectable } from '@angular/core';
import { ApiEndPoints } from '../shared/config/api-end-points';
import { PagingConstant } from '../shared/constants/constants';
import { HttpBaseService } from '../shared/services/http-base.service';
import { Cage } from '../models/cage.model';
import { PaginationResponse } from '../models/pagination-response.model';

@Injectable({
  providedIn: "root",
})
export class CageService {
  constructor(
    private api: ApiEndPoints,
    private readonly _httpService: HttpBaseService
  ) { }

  async getCages(sortField: string, sortDirection = "asc", pageIndex = PagingConstant.pageIndex, pageSize = PagingConstant.pageSize): Promise<PaginationResponse<Cage> | undefined> {
    return await this._httpService.getDataAsync<PaginationResponse<Cage>>(this.api.getCagesWithPaging(sortField, sortDirection, pageIndex, pageSize));
  }

  async getCageById(cageId: string) : Promise<Cage | undefined> {
    return await this._httpService.getDataAsync<Cage>(this.api.getCageByCageId(cageId));
  }

  async createCage(cage: Cage) : Promise<Cage | undefined> {
    return await this._httpService.postDataAsync<Cage>(this.api.createCage(), cage);
  }

  async updateCage(data: Cage): Promise<Cage | undefined> {
    return await this._httpService.putDataAsync<Cage>(
      this.api.updateCage(),
      data
    );
  }
}
