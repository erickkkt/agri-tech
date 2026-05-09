export enum VaccineStatus {
  Scheduled = 0,
  Administered = 1,
  Missed = 2,
  Cancelled = 3
}

export const VaccineStatusLabels: Record<VaccineStatus, string> = {
  [VaccineStatus.Scheduled]: 'Đã lên lịch',
  [VaccineStatus.Administered]: 'Đã tiêm',
  [VaccineStatus.Missed]: 'Quá hạn',
  [VaccineStatus.Cancelled]: 'Đã hủy'
};
