using Farm.Domain.Enum;

namespace Farm.Domain.ViewModels.Alert
{
    public class AlertDto
    {
        public Guid Id { get; set; }
        public AlertType Type { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public Guid? FarmId { get; set; }
        public string FarmName { get; set; }
        public Guid? AnimalId { get; set; }
        public string AnimalCode { get; set; }
        public Guid? FeedItemId { get; set; }
        public string FeedItemName { get; set; }
        public Guid? VaccineScheduleId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Payload { get; set; }
    }

    public class AlertSummaryDto
    {
        public int TotalUnread { get; set; }
        public int CriticalUnread { get; set; }
        public int WarningUnread { get; set; }
    }
}
