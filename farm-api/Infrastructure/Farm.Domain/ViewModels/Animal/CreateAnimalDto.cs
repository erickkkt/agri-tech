using Farm.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Farm.Domain.ViewModels.Animal
{
    public class CreateAnimalDto
    {
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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid CageId { get; set; }
        public Guid FarmId { get; set; }
    }
}
