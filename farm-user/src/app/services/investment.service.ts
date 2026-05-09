import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { AnimalUpdate, InvestmentOffer, InvestmentOrder } from '../models/investment.model';

@Injectable({ providedIn: 'root' })
export class InvestmentService {
  private base = `${environment.apiBaseUrl}/api/v1/investment`;

  constructor(private http: HttpClient) { }

  getOpenOffers(pageIndex = 0, pageSize = 20) {
    return this.http.get<InvestmentOffer[]>(`${this.base}/offers`, { params: { pageIndex, pageSize } as any }).toPromise();
  }

  getOffer(id: string) {
    return this.http.get<InvestmentOffer>(`${this.base}/offers/${id}`).toPromise();
  }

  placeOrder(offerId: string, shareQty: number) {
    return this.http.post<string>(`${this.base}/orders`, { offerId, shareQty }).toPromise();
  }

  myOrders() {
    return this.http.get<InvestmentOrder[]>(`${this.base}/orders/me`).toPromise();
  }

  getAnimalUpdates(animalId: string, take = 50) {
    return this.http.get<AnimalUpdate[]>(`${this.base}/animal-updates/${animalId}`, { params: { take } as any }).toPromise();
  }
}
