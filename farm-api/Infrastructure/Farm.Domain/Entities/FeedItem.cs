using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    /// <summary>
    /// A feed product / catalog item used at a farm (e.g. starter feed, finisher feed).
    /// Inventory level is computed from FeedTransaction entries.
    /// </summary>
    public class FeedItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, StringLength(50)]
        public string Code { get; set; }

        [Required, StringLength(250)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Unit { get; set; } = "kg";

        [StringLength(250)]
        public string Manufacturer { get; set; }

        [StringLength(2000)]
        public string NutritionInfo { get; set; }

        /// <summary>
        /// When stock falls below this level, FeedLowStockJob raises an alert.
        /// </summary>
        public float LowStockThreshold { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
