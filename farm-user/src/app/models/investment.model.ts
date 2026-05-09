export enum InvestmentOfferStatus {
  Draft = 0,
  Open = 1,
  Closed = 2,
  Harvested = 3
}

export enum InvestmentOrderStatus {
  Pending = 0,
  Confirmed = 1,
  Cancelled = 2,
  Refunded = 3
}

export interface InvestmentOffer {
  id: string;
  animalId: string;
  farmId: string;
  title: string;
  description: string;
  totalShares: number;
  availableShares: number;
  pricePerShare: number;
  profitRatio: number;
  expectedHarvestDate?: Date;
  status: InvestmentOfferStatus;
}

export interface InvestmentOrder {
  id: string;
  offerId: string;
  investorUserId: string;
  shareQty: number;
  totalAmount: number;
  status: InvestmentOrderStatus;
  createdAt: Date;
}

export interface AnimalUpdate {
  id: string;
  animalId: string;
  authorUserName: string;
  updateType: number;
  recordedAt: Date;
  description: string;
  mediaUrls: string;
}
