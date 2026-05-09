using Farm.Domain.Attibutes;
using Farm.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    [TrackAudit]
    public class Animal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, StringLength(10)]
        public string Code { get; set; }

        [Required, StringLength(250)]
        public string Name { get; set; }
        
        public Species Species { get; set; }
        public Gender Gender { get; set; }
        public HealthStatus HealthStatus { get; set; }

        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfArrival { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }

        public float Weight { get; set; }
        public float Height { get; set; }
        
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; }
        public DateTime? ChangedAt { get; set; }

        public Guid CageId { get; set; }
        [ForeignKey("CageId")]
        public virtual Cage Cage { get; set; }                     
    }
}
