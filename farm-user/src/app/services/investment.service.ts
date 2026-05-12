import { Injectable } from '@angular/core';
import { HttpBaseService } from '../shared/services/http-base.service';
import { ApiEndPoints } from '../shared/config/api-end-points';
import { AnimalUpdate, InvestmentOffer, InvestmentOrder } from '../models/investment.model';

@Injectable({ providedIn: 'root' })
export class InvestmentService {

  constructor(
    private readonly http: HttpBaseService,
    private readonly api: ApiEndPoints
  ) { }

  getOpenOffers(pageIndex = 0, pageSize = 20): Promise<InvestmentOffer[] | undefined> {
    return this.http.getDataAsync<InvestmentOffer[]>(this.api.getInvestmentOffers(), { pageIndex, pageSize });
  }

  getOffer(id: string): Promise<InvestmentOffer | undefined> {
    return this.http.getDataAsync<InvestmentOffer>(this.api.getInvestmentOffer(id));
  }

  placeOrder(offerId: string, shareQty: number): Promise<string | undefined> {
    return this.http.postDataAsync<string>(this.api.placeInvestmentOrder(), { offerId, shareQty });
  }

  myOrders(): Promise<InvestmentOrder[] | undefined> {
    return this.http.getDataAsync<InvestmentOrder[]>(this.api.getMyInvestmentOrders());
  }

  getAnimalUpdates(animalId: string, take = 50): Promise<AnimalUpdate[] | undefined> {
    return this.http.getDataAsync<AnimalUpdate[]>(this.api.getAnimalUpdates(animalId), { take });
  }
}
