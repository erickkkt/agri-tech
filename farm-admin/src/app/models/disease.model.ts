export enum DiseaseSeverity {
  Mild = 0,
  Moderate = 1,
  Severe = 2,
  Critical = 3
}

export enum DiseaseStatus {
  Active = 0,
  UnderTreatment = 1,
  Recovered = 2,
  Fatal = 3
}

export interface DiseaseRecord {
  id: string;
  animalId: string;
  animalCode: string;
  animalName: string;
  diseaseName: string;
  diagnosedAt: Date;
  diagnosedBy: string;
  severity: DiseaseSeverity;
  status: DiseaseStatus;
  recoveredAt?: Date;
  notes: string;
}

export interface Treatment {
  id: string;
  animalId: string;
  diseaseRecordId?: string;
  diseaseName: string;
  medication: string;
  dosage: string;
  startDate: Date;
  endDate?: Date;
  administeredBy: string;
  outcome: string;
}
