export enum ListingCategory {
  Breeding = 0,
  Antler = 1,
  Meat = 2,
  Egg = 3,
  Other = 99
}

export enum ListingStatus {
  Draft = 0,
  Active = 1,
  Sold = 2,
  Closed = 3
}

export interface Listing {
  id: string;
  farmId: string;
  farmName: string;
  sellerUserId: string;
  title: string;
  description: string;
  category: ListingCategory;
  species: number;
  status: ListingStatus;
  price: number;
  currency: string;
  quantity: number;
  unit: string;
  province: string;
  createdAt: Date;
  photoUrls: string[];
}

export interface ListingSearchQuery {
  q?: string;
  province?: string;
  species?: number;
  category?: ListingCategory;
  pageIndex?: number;
  pageSize?: number;
}
