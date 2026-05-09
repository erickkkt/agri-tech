import { Species } from './enum/species.enum';
import { VaccineStatus } from './enum/vaccine-status.enum';

export interface Vaccine {
  id: string;
  name: string;
  manufacturer: string;
  recommendedSpecies?: Species;
  intervalDays: number;
  description: string;
  isActive: boolean;
}

export interface VaccineSchedule {
  id: string;
  animalId: string;
  animalCode: string;
  animalName: string;
  vaccineId: string;
  vaccineName: string;
  scheduledDate: Date;
  administeredDate?: Date;
  administeredBy: string;
  status: VaccineStatus;
  notes: string;
}

export interface CreateVaccineSchedule {
  animalId: string;
  vaccineId: string;
  scheduledDate: Date;
  notes?: string;
}

export interface AdministerVaccine {
  scheduleId: string;
  administeredDate: Date;
  administeredBy: string;
  notes?: string;
}
