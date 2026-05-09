using Farm.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    /// <summary>
    /// Catalog of vaccine types that can be administered to animals.
    /// </summary>
    public class Vaccine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, StringLength(250)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Species this vaccine is recommended for (nullable = applies to all).
        /// </summary>
        public Species? RecommendedSpecies { get; set; }

        /// <summary>
        /// Suggested re-vaccination interval in days (used by Hangfire reminder job).
        /// </summary>
        public int IntervalDays { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
