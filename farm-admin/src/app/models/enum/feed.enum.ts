export enum FeedTransactionType {
  In = 0,
  Out = 1,
  Adjustment = 2,
  Loss = 3
}

export const FeedTransactionTypeLabels: Record<FeedTransactionType, string> = {
  [FeedTransactionType.In]: 'Nhập kho',
  [FeedTransactionType.Out]: 'Xuất kho',
  [FeedTransactionType.Adjustment]: 'Điều chỉnh',
  [FeedTransactionType.Loss]: 'Hao hụt'
};
