
using System.ComponentModel.DataAnnotations;

namespace Farm.Domain.ViewModels.Farm
{
    public class CreateFarmDto
    {
        [Required, StringLength(250)]
        public string Name { get; set; }

        [StringLength(250)]
        public string OwnerName { get; set; }

        [StringLength(400)]
        public string Description { get; set; }

        [StringLength(4000)]
        public string Location { get; set; }

        // Bank info — receiving account for marketplace sales + investor transfers
        [StringLength(120)] public string BankName { get; set; }
        [StringLength(50)]  public string BankAccountNumber { get; set; }
        [StringLength(250)] public string BankAccountHolder { get; set; }
        [StringLength(120)] public string BankBranch { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; }
    }
}
