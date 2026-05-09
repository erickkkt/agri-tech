using Farm.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Farm.Domain.ViewModels.Feed
{
    public class FeedItemDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Manufacturer { get; set; }
        public string NutritionInfo { get; set; }
        public float LowStockThreshold { get; set; }
        public bool IsActive { get; set; }
        public float CurrentStock { get; set; }
    }

    public class CreateFeedItemDto
    {
        [Required, StringLength(50)] public string Code { get; set; }
        [Required, StringLength(250)] public string Name { get; set; }
        [StringLength(50)] public string Unit { get; set; } = "kg";
        [StringLength(250)] public string Manufacturer { get; set; }
        [StringLength(2000)] public string NutritionInfo { get; set; }
        [Range(0, float.MaxValue)] public float LowStockThreshold { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateFeedItemDto : CreateFeedItemDto
    {
        [Required] public Guid Id { get; set; }
    }

    public class FeedTransactionDto
    {
        public Guid Id { get; set; }
        public Guid FeedItemId { get; set; }
        public string FeedItemName { get; set; }
        public Guid FarmId { get; set; }
        public string FarmName { get; set; }
        public FeedTransactionType Type { get; set; }
        public float Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Notes { get; set; }
    }

    public class CreateFeedTransactionDto
    {
        [Required] public Guid FeedItemId { get; set; }
        [Required] public Guid FarmId { get; set; }
        [Required] public FeedTransactionType Type { get; set; }
        [Range(0.0001, float.MaxValue)] public float Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        [Required] public DateTime TransactionDate { get; set; }
        [StringLength(2000)] public string Notes { get; set; }
    }

    public class FeedConsumptionDto
    {
        public Guid Id { get; set; }
        public Guid FeedItemId { get; set; }
        public string FeedItemName { get; set; }
        public Guid? AnimalId { get; set; }
        public Guid? CageId { get; set; }
        public DateTime ConsumedAt { get; set; }
        public float Quantity { get; set; }
        public string GrowthStage { get; set; }
        public string Notes { get; set; }
    }

    public class CreateFeedConsumptionDto
    {
        [Required] public Guid FeedItemId { get; set; }
        public Guid? AnimalId { get; set; }
        public Guid? CageId { get; set; }
        [Required] public DateTime ConsumedAt { get; set; }
        [Range(0.0001, float.MaxValue)] public float Quantity { get; set; }
        [StringLength(50)] public string GrowthStage { get; set; }
        [StringLength(1000)] public string Notes { get; set; }
    }

    public class FeedSummaryDto
    {
        public Guid FarmId { get; set; }
        public string FarmName { get; set; }
        public List<FeedStockLevelDto> Items { get; set; } = new();
    }

    public class FeedStockLevelDto
    {
        public Guid FeedItemId { get; set; }
        public string FeedItemCode { get; set; }
        public string FeedItemName { get; set; }
        public string Unit { get; set; }
        public float TotalIn { get; set; }
        public float TotalOut { get; set; }
        public float Stock { get; set; }
        public float LowStockThreshold { get; set; }
        public bool IsLowStock => Stock <= LowStockThreshold;
    }
}
