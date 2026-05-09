export interface GrowthLog {
  id: string;
  animalId: string;
  animalCode: string;
  animalName: string;
  recordedAt: Date;
  weight: number;
  height?: number;
  bodyConditionScore?: number;
  photoUrl: string;
  notes: string;
}

export interface CreateGrowthLog {
  animalId: string;
  recordedAt: Date;
  weight: number;
  height?: number;
  bodyConditionScore?: number;
  photoUrl?: string;
  notes?: string;
}
