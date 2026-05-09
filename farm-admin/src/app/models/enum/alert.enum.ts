export enum AlertType {
  MissedVaccine = 0,
  VaccineReminder = 1,
  WeightDrop = 2,
  StagnantGrowth = 3,
  DiseaseDetected = 4,
  FeedLowStock = 5,
  System = 6
}

export const AlertTypeLabels: Record<AlertType, string> = {
  [AlertType.MissedVaccine]: 'Tiêm vaccine trễ',
  [AlertType.VaccineReminder]: 'Nhắc tiêm vaccine',
  [AlertType.WeightDrop]: 'Giảm cân bất thường',
  [AlertType.StagnantGrowth]: 'Không tăng trưởng',
  [AlertType.DiseaseDetected]: 'Phát hiện bệnh',
  [AlertType.FeedLowStock]: 'Tồn kho thức ăn thấp',
  [AlertType.System]: 'Hệ thống'
};

export enum AlertSeverity {
  Info = 0,
  Warning = 1,
  Critical = 2
}

export const AlertSeverityLabels: Record<AlertSeverity, string> = {
  [AlertSeverity.Info]: 'Thông tin',
  [AlertSeverity.Warning]: 'Cảnh báo',
  [AlertSeverity.Critical]: 'Nghiêm trọng'
};
