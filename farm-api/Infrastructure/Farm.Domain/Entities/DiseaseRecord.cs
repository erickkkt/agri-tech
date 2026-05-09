using Farm.Domain.Attibutes;
using Farm.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    /// <summary>
    /// Disease occurrence on a single animal.
    /// </summary>
    [TrackAudit]
    public class DiseaseRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; }

        [Required, StringLength(250)]
        public string DiseaseName { get; set; }

        public DateTime DiagnosedAt { get; set; }

        [StringLength(250)]
        public string DiagnosedBy { get; set; }

        public DiseaseSeverity Severity { get; set; }
        public DiseaseStatus Status { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        public DateTime? RecoveredAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; }
        public DateTime? ChangedAt { get; set; }

        public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
    }
}
