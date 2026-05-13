
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    public class Farm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, StringLength(250)]
        public string Name { get; set; }

        [StringLength(250)]
        public string OwnerName { get; set; }

        [StringLength(400)]
        public string Description { get; set; }

        [StringLength(4000)]
        public string Location { get; set; }

        // Bank account info — receiving account for marketplace sales and
        // investor transfers. Nullable because legacy farms may not have it set.
        [StringLength(120)] public string BankName { get; set; }
        [StringLength(50)]  public string BankAccountNumber { get; set; }
        [StringLength(250)] public string BankAccountHolder { get; set; }
        [StringLength(120)] public string BankBranch { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        
        // Navigation properties
        public virtual ICollection<Cage> Cages { get; set; } = new List<Cage>();
    }
}
