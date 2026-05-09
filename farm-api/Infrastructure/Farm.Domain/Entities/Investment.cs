using Farm.Domain.Attibutes;
using Farm.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    /// <summary>
    /// A farm posts an animal (or batch backed by an animal) for share-based investment.
    /// Total shares are split among investors; profit at harvest is distributed proportionally.
    /// </summary>
    [TrackAudit]
    public class InvestmentOffer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; }

        public Guid FarmId { get; set; }
        [ForeignKey("FarmId")]
        public virtual Farm Farm { get; set; }

        [Required, StringLength(250)]
        public string Title { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }

        public int TotalShares { get; set; }
        public int AvailableShares { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerShare { get; set; }

        /// <summary>Investor profit share (0..1). e.g. 0.7 = investors get 70% of harvest revenue.</summary>
        [Column(TypeName = "decimal(5,4)")]
        public decimal ProfitRatio { get; set; }

        public DateTime? ExpectedHarvestDate { get; set; }
        public InvestmentOfferStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; }
        public DateTime? ChangedAt { get; set; }
    }

    [TrackAudit]
    public class InvestmentOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid OfferId { get; set; }
        [ForeignKey("OfferId")]
        public virtual InvestmentOffer Offer { get; set; }

        public Guid InvestorUserId { get; set; }
        [StringLength(250)]
        public string InvestorUserName { get; set; }

        public int ShareQty { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public InvestmentOrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; }
        public DateTime? ChangedAt { get; set; }
    }

    public class ShareCertificate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual InvestmentOrder Order { get; set; }

        [StringLength(50)]
        public string CertificateNo { get; set; }

        public DateTime IssuedAt { get; set; }

        [StringLength(1000)]
        public string ContractDocumentUrl { get; set; }
    }

    /// <summary>Daily / event-driven update on an animal, visible to investors who hold shares.</summary>
    public class AnimalUpdate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; }

        public Guid AuthorUserId { get; set; }
        [StringLength(250)]
        public string AuthorUserName { get; set; }

        public AnimalUpdateType UpdateType { get; set; }
        public DateTime RecordedAt { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        /// <summary>CSV / JSON of media URLs (photos, video clips).</summary>
        [StringLength(4000)]
        public string MediaUrls { get; set; }
    }

    public class CameraFeed
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid? CageId { get; set; }
        [ForeignKey("CageId")]
        public virtual Cage Cage { get; set; }

        public Guid? FarmId { get; set; }
        [ForeignKey("FarmId")]
        public virtual Farm Farm { get; set; }

        [Required, StringLength(250)]
        public string Name { get; set; }

        /// <summary>RTSP / HLS / WebRTC URL. Sensitive — restricted by AccessRoles.</summary>
        [Required, StringLength(2000)]
        public string StreamUrl { get; set; }

        public bool IsActive { get; set; }

        /// <summary>Comma-separated role names allowed to access (e.g. "FARM.OWNER,INVESTOR").</summary>
        [StringLength(500)]
        public string AccessRoles { get; set; }
    }

    /// <summary>Records an actual harvest event (antler cut, slaughter, egg collection batch).</summary>
    public class HarvestEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; }

        public Guid? OfferId { get; set; }
        [ForeignKey("OfferId")]
        public virtual InvestmentOffer Offer { get; set; }

        public HarvestType HarvestType { get; set; }
        public DateTime HarvestDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GrossRevenue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class ProfitDistribution
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid HarvestEventId { get; set; }
        [ForeignKey("HarvestEventId")]
        public virtual HarvestEvent HarvestEvent { get; set; }

        public Guid InvestorUserId { get; set; }
        public Guid OrderId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime CalculatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending / Paid / Failed
    }
}
