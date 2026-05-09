using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    public class Cage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, StringLength(250)]
        public string Name { get; set; }

        [StringLength(400)]
        public string Description { get; set; }

        public int Capacity { get; set; }
        public int CurrentAnimalCount { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }


        public Guid FarmId { get; set; }

        [ForeignKey("FarmId")]
        public virtual Farm Farm { get; set; }
    }
}
