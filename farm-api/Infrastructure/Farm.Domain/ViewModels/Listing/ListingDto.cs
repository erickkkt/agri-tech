using Farm.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Farm.Domain.ViewModels.Listing
{
    public class ListingDto
    {
        public Guid Id { get; set; }
        public Guid FarmId { get; set; }
        public string FarmName { get; set; }
        public Guid SellerUserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ListingCategory Category { get; set; }
        public Species Species { get; set; }
        public ListingStatus Status { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string Province { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> PhotoUrls { get; set; } = new();
    }

    public class CreateListingDto
    {
        [Required] public Guid FarmId { get; set; }
        [Required, StringLength(250)] public string Title { get; set; }
        [StringLength(4000)] public string Description { get; set; }
        public ListingCategory Category { get; set; }
        public Species Species { get; set; }
        [Range(0, double.MaxValue)] public decimal Price { get; set; }
        [StringLength(20)] public string Currency { get; set; } = "VND";
        [Range(1, int.MaxValue)] public int Quantity { get; set; }
        [StringLength(50)] public string Unit { get; set; } = "con";
        [StringLength(120)] public string Province { get; set; }
        public List<string> PhotoUrls { get; set; } = new();
    }

    public class UpdateListingDto : CreateListingDto
    {
        [Required] public Guid Id { get; set; }
        public ListingStatus Status { get; set; }
    }

    public class ListingSearchDto
    {
        public string Q { get; set; }
        public string Province { get; set; }
        public Species? Species { get; set; }
        public ListingCategory? Category { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 20;
    }
}
