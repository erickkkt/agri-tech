import { Injectable } from '@angular/core';
import { ApiEndPoints } from '../shared/config/api-end-points';
import { PagingConstant } from '../shared/constants/constants';
import { HttpBaseService } from '../shared/services/http-base.service';
import { Animal } from '../models/animal.model';
import { PaginationResponse } from '../models/pagination-response.model';

@Injectable({
  providedIn: "root",
})
export class AnimalService {
  constructor(
    private api: ApiEndPoints,
    private readonly _httpService: HttpBaseService
  ) { }

  async getAnimals(sortField: string, sortDirection = "asc", pageIndex = PagingConstant.pageIndex, pageSize = PagingConstant.pageSize): Promise<PaginationResponse<Animal> | undefined> {
    return await this._httpService.getDataAsync<PaginationResponse<Animal>>(this.api.getAnimalsWithPaging(sortField, sortDirection, pageIndex, pageSize));
  }

  async getAnimalById(animalId: string) : Promise<Animal | undefined> {
    return await this._httpService.getDataAsync<Animal>(this.api.getAnimalByAnimalId(animalId));
  }

  async createAnimal(animal: Animal) : Promise<Animal | undefined> {
    return await this._httpService.postDataAsync<Animal>(this.api.createAnimal(), animal);
  }

  async updateAnimal(data: Animal): Promise<Animal | undefined> {
    return await this._httpService.putDataAsync<Animal>(
      this.api.updateAnimal(),
      data
    );
  }
}
