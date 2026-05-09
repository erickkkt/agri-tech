using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    /// <summary>
    /// Records the daily feed consumption either per animal or per cage,
    /// used to drive efficiency reports and growth-stage analytics.
    /// </summary>
    public class FeedConsumption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid FeedItemId { get; set; }
        [ForeignKey("FeedItemId")]
        public virtual FeedItem FeedItem { get; set; }

        public Guid? AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; }

        public Guid? CageId { get; set; }
        [ForeignKey("CageId")]
        public virtual Cage Cage { get; set; }

        public DateTime ConsumedAt { get; set; }

        public float Quantity { get; set; }

        /// <summary>e.g. "Newborn", "Growing", "Finishing", "Adult".</summary>
        [StringLength(50)]
        public string GrowthStage { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
