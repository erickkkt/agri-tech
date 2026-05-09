import { FeedTransactionType } from './enum/feed.enum';

export interface FeedItem {
  id: string;
  code: string;
  name: string;
  unit: string;
  manufacturer: string;
  nutritionInfo: string;
  lowStockThreshold: number;
  isActive: boolean;
  currentStock?: number;
}

export interface FeedTransaction {
  id: string;
  feedItemId: string;
  feedItemName: string;
  farmId: string;
  farmName: string;
  type: FeedTransactionType;
  quantity: number;
  unitPrice?: number;
  transactionDate: Date;
  notes: string;
}

export interface CreateFeedTransaction {
  feedItemId: string;
  farmId: string;
  type: FeedTransactionType;
  quantity: number;
  unitPrice?: number;
  transactionDate: Date;
  notes?: string;
}

export interface FeedConsumption {
  id: string;
  feedItemId: string;
  feedItemName: string;
  animalId?: string;
  cageId?: string;
  consumedAt: Date;
  quantity: number;
  growthStage: string;
  notes: string;
}

export interface FeedSummary {
  farmId: string;
  farmName: string;
  items: FeedStockLevel[];
}

export interface FeedStockLevel {
  feedItemId: string;
  feedItemCode: string;
  feedItemName: string;
  unit: string;
  totalIn: number;
  totalOut: number;
  stock: number;
  lowStockThreshold: number;
  isLowStock: boolean;
}
