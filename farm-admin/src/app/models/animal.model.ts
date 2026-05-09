import { Gender } from "./enum/gender.enum";
import { HealthStatus } from "./enum/health-status.enum";
import { Species } from "./enum/species.enum";

export interface Animal {
    id: string;
    name: string;
    species: Species;
    code: string;
    height: number;
    weight: number;
    gender: Gender;
    cageId: string;
    cageName: string;
    farmId: string;
    farmName: string;
    healthStatus: HealthStatus;
    description: string;
    isActive: boolean;
    dateOfBirth: Date;
    dateOfArrival: Date;
}
