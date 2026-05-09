
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    public class Audit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(255)]
        [Required]
        public string EntityName { get; set; }

        [MaxLength(255)]
        [Required]
        public string PropertyName { get; set; }

        [Required]
        public Guid PrimaryKeyValue { get; set; }

        public string OriginalValue { get; set; }

        public string CurrentValue { get; set; }

        [Required]
        public Guid ChangedByUserId { get; set; }

        [Required, MaxLength(250)]
        public string ChangedByUserName { get; set; }

        [Required]
        public DateTime ChangedDateUtc { get; set; }
    }
}
