using Farm.Domain.Attibutes;
using Farm.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    /// <summary>
    /// Marketplace listing - a farm posting items for sale.
    /// </summary>
    [TrackAudit]
    public class Listing
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid FarmId { get; set; }
        [ForeignKey("FarmId")]
        public virtual Farm Farm { get; set; }

        public Guid SellerUserId { get; set; }

        [Required, StringLength(250)]
        public string Title { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }

        public ListingCategory Category { get; set; }
        public Species Species { get; set; }
        public ListingStatus Status { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(20)]
        public string Currency { get; set; } = "VND";

        public int Quantity { get; set; }
        [StringLength(50)]
        public string Unit { get; set; } = "con";

        [StringLength(120)]
        public string Province { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; }
        public DateTime? ChangedAt { get; set; }

        public virtual ICollection<ListingPhoto> Photos { get; set; } = new List<ListingPhoto>();
    }

    public class ListingPhoto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid ListingId { get; set; }
        [ForeignKey("ListingId")]
        public virtual Listing Listing { get; set; }

        [Required, StringLength(1000)]
        public string Url { get; set; }

        public int Order { get; set; }
    }
}
