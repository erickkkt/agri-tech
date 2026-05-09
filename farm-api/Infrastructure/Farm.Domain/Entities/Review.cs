using Farm.Domain.Attibutes;
using Farm.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    /// <summary>1-5 star review of a Farm by a buyer/investor user.</summary>
    [TrackAudit]
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid TargetFarmId { get; set; }
        [ForeignKey("TargetFarmId")]
        public virtual Farm TargetFarm { get; set; }

        public Guid ReviewerUserId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(2000)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; }
        public DateTime? ChangedAt { get; set; }
    }

    public class FarmVerification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid FarmId { get; set; }
        [ForeignKey("FarmId")]
        public virtual Farm Farm { get; set; }

        public FarmVerificationStatus Status { get; set; }

        [StringLength(2000)]
        public string DocumentUrls { get; set; } // CSV / JSON of document URLs

        public DateTime SubmittedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public Guid? VerifiedByUserId { get; set; }

        [StringLength(2000)]
        public string ReviewerNotes { get; set; }
    }

    public class ShippingPartner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, StringLength(250)]
        public string Name { get; set; }

        [StringLength(2000)]
        public string ServiceArea { get; set; }

        /// <summary>CSV of supported species (e.g. "Deer,Sheep").</summary>
        [StringLength(500)]
        public string AnimalTypesSupported { get; set; }

        [StringLength(500)]
        public string ContactInfo { get; set; }

        public bool IsActive { get; set; }
    }
}
