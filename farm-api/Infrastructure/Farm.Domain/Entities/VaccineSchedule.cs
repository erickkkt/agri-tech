using Farm.Domain.Attibutes;
using Farm.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    /// <summary>
    /// Represents a single vaccination event scheduled or completed for an animal.
    /// </summary>
    [TrackAudit]
    public class VaccineSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; }

        public Guid VaccineId { get; set; }
        [ForeignKey("VaccineId")]
        public virtual Vaccine Vaccine { get; set; }

        public DateTime ScheduledDate { get; set; }

        public DateTime? AdministeredDate { get; set; }

        [StringLength(250)]
        public string AdministeredBy { get; set; }

        public VaccineStatus Status { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        /// <summary>True after VaccineReminderJob has issued an alert for this schedule.</summary>
        public bool ReminderSent { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; }
        public DateTime? ChangedAt { get; set; }
    }
}
