export interface Dashboard {
  totalFarms: number;
  totalCages: number;
  totalAnimals: number;
  healthyAnimals: number;
  sickAnimals: number;
  vaccineSchedulesUpcoming: number;
  vaccineSchedulesOverdue: number;
  feedItemsLowStock: number;
  unreadAlerts: number;
  criticalAlerts: number;
  animalsBySpecies: { species: string; count: number }[];
  animalsByHealth: { healthStatus: string; count: number }[];
  averageGrowthByMonth: { year: number; month: number; averageWeight: number; logsCount: number }[];
}
