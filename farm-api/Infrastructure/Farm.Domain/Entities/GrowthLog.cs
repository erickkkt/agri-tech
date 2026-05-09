using Farm.Domain.Attibutes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    /// <summary>
    /// Periodic measurement of an animal: weight, height, body condition.
    /// Multiple entries per animal are expected; ordered by RecordedAt.
    /// </summary>
    [TrackAudit]
    public class GrowthLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; }

        public DateTime RecordedAt { get; set; }

        public float Weight { get; set; }
        public float? Height { get; set; }

        /// <summary>Body condition score (1-9 typical scale).</summary>
        public int? BodyConditionScore { get; set; }

        [StringLength(1000)]
        public string PhotoUrl { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; }
        public DateTime? ChangedAt { get; set; }
    }
}
