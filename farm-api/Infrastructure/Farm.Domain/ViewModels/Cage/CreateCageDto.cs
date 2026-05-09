using System.ComponentModel.DataAnnotations;

namespace Farm.Domain.ViewModels.Cage
{
    public class CreateCageDto
    {
        [Required, StringLength(250)]
        public string Name { get; set; }

        [StringLength(400)]
        public string Description { get; set; }

        public int Capacity { get; set; }
        public int CurrentAnimalCount { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid FarmId { get; set; }
    }

}
