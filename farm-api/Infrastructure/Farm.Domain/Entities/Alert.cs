using Farm.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    /// <summary>
    /// System / business alert raised by Hangfire jobs or by services.
    /// Pushed to clients in realtime via SignalR (NotificationHub).
    /// </summary>
    public class Alert
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public AlertType Type { get; set; }
        public AlertSeverity Severity { get; set; }

        [Required, StringLength(500)]
        public string Title { get; set; }

        [Required, StringLength(2000)]
        public string Message { get; set; }

        public Guid? FarmId { get; set; }
        [ForeignKey("FarmId")]
        public virtual Farm Farm { get; set; }

        public Guid? AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; }

        public Guid? FeedItemId { get; set; }
        [ForeignKey("FeedItemId")]
        public virtual FeedItem FeedItem { get; set; }

        public Guid? VaccineScheduleId { get; set; }
        [ForeignKey("VaccineScheduleId")]
        public virtual VaccineSchedule VaccineSchedule { get; set; }

        /// <summary>JSON payload with extra context for the UI (optional).</summary>
        [StringLength(4000)]
        public string Payload { get; set; }

        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public Guid? ReadByUserId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
