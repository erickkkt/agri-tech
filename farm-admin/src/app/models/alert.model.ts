import { AlertSeverity, AlertType } from './enum/alert.enum';

export interface Alert {
  id: string;
  type: AlertType;
  severity: AlertSeverity;
  title: string;
  message: string;
  farmId?: string;
  farmName?: string;
  animalId?: string;
  animalCode?: string;
  feedItemId?: string;
  feedItemName?: string;
  vaccineScheduleId?: string;
  isRead: boolean;
  readAt?: Date;
  createdAt: Date;
  payload?: string;
}

export interface AlertSummary {
  totalUnread: number;
  criticalUnread: number;
  warningUnread: number;
}
