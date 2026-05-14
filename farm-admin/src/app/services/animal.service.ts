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

  // ---------- Sell / Invest ----------

  async getPriceSuggestion(animalId: string): Promise<PriceSuggestion | undefined> {
    return await this._httpService.getDataAsync<PriceSuggestion>(this.api.getAnimalPriceSuggestion(animalId));
  }

  async listForSale(animalId: string, dto: ListForSaleDto): Promise<string | undefined> {
    return await this._httpService.postDataAsync<string>(this.api.listAnimalForSale(animalId), dto);
  }

  async openInvestment(animalId: string, dto: OpenInvestmentDto): Promise<string | undefined> {
    return await this._httpService.postDataAsync<string>(this.api.openAnimalInvestment(animalId), dto);
  }

  async getSalesHistory(animalId: string): Promise<AnimalSaleHistoryItem[] | undefined> {
    return await this._httpService.getDataAsync<AnimalSaleHistoryItem[]>(this.api.getAnimalSalesHistory(animalId));
  }

  async getInvestmentHistory(animalId: string): Promise<AnimalInvestmentHistoryItem[] | undefined> {
    return await this._httpService.getDataAsync<AnimalInvestmentHistoryItem[]>(this.api.getAnimalInvestmentHistory(animalId));
  }

  /** Update an existing Listing — used to change status (Active → Sold → Closed). */
  async updateListing(payload: UpdateListingPayload): Promise<void> {
    await this._httpService.putDataAsync<unknown>(this.api.updateListing(), payload);
  }
}

export interface UpdateListingPayload {
  id: string;
  title: string;
  description?: string;
  category: number;
  species: number;
  status: number;            // ListingStatus
  price: number;
  currency: string;
  quantity: number;
  unit: string;
  province?: string;
  photoUrls?: string[];
}

// ---------- DTOs (mirror backend) ----------

export interface PriceSuggestion {
  weight: number;
  species: number;
  pricePerKg: number;
  suggestedPrice: number;
  currency: string;
}

export interface ListForSaleDto {
  farmId?: string;
  title?: string;
  description?: string;
  category: number;           // ListingCategory enum
  price: number;
  currency?: string;
  quantity?: number;
  unit?: string;
  province?: string;
  photoUrls?: string[];
}

export interface OpenInvestmentDto {
  farmId?: string;
  title?: string;
  description?: string;
  totalShares: number;
  pricePerShare: number;
  profitRatio: number;        // 0..1
  expectedHarvestDate?: string; // ISO
}

export interface AnimalSaleHistoryItem {
  id: string;
  title: string;
  status: number;             // ListingStatus
  category: number;
  price: number;
  currency: string;
  quantity: number;
  unit: string;
  createdAt: string;
}

export interface AnimalInvestmentHistoryItem {
  id: string;
  title: string;
  status: number;             // InvestmentOfferStatus
  totalShares: number;
  availableShares: number;
  pricePerShare: number;
  profitRatio: number;
  expectedHarvestDate?: string;
  createdAt: string;
}
