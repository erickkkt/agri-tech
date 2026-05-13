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

  getCommitment(orderId: string): Promise<CommitmentDto | undefined> {
    return this.http.getDataAsync<CommitmentDto>(this.api.getInvestmentCommitment(orderId));
  }
}

// ---------- Commitment ----------

export interface CommitmentDto {
  orderId: string;
  createdAt: string;
  status: string;
  transferReference: string;
  bankTransferConfirmedAt?: string;

  totalAmount: number;
  currency: string;
  profitRatio: number;
  expectedHarvestDate?: string;
  offerTitle: string;
  offerDescription: string;

  investor: { userId: string; name: string };
  farm: {
    id: string; name: string; ownerName: string; location: string;
    bankName: string; bankAccountNumber: string; bankAccountHolder: string; bankBranch: string;
  };
  animal: {
    id: string; code: string; name: string; species: string; weight: number; dateOfBirth?: string;
  };
}
