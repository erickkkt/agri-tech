using System.ComponentModel.DataAnnotations;

namespace Farm.Domain.ViewModels.GrowthLog
{
    public class GrowthLogDto
    {
        public Guid Id { get; set; }
        public Guid AnimalId { get; set; }
        public string AnimalCode { get; set; }
        public string AnimalName { get; set; }
        public DateTime RecordedAt { get; set; }
        public float Weight { get; set; }
        public float? Height { get; set; }
        public int? BodyConditionScore { get; set; }
        public string PhotoUrl { get; set; }
        public string Notes { get; set; }
    }

    public class CreateGrowthLogDto
    {
        [Required] public Guid AnimalId { get; set; }
        [Required] public DateTime RecordedAt { get; set; }
        [Range(0, 100000)] public float Weight { get; set; }
        public float? Height { get; set; }
        [Range(1, 9)] public int? BodyConditionScore { get; set; }
        [StringLength(1000)] public string PhotoUrl { get; set; }
        [StringLength(2000)] public string Notes { get; set; }
    }

    public class UpdateGrowthLogDto : CreateGrowthLogDto
    {
        [Required] public Guid Id { get; set; }
    }
}
