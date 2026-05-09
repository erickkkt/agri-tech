using Farm.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    /// <summary>
    /// Stock-in / stock-out / adjustment movement for a feed item at a farm.
    /// </summary>
    public class FeedTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid FeedItemId { get; set; }
        [ForeignKey("FeedItemId")]
        public virtual FeedItem FeedItem { get; set; }

        public Guid FarmId { get; set; }
        [ForeignKey("FarmId")]
        public virtual Farm Farm { get; set; }

        public FeedTransactionType Type { get; set; }

        public float Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? UnitPrice { get; set; }

        public DateTime TransactionDate { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }
    }
}
