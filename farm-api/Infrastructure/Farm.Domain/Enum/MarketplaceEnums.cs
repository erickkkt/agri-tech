using System.ComponentModel;

namespace Farm.Domain.Enum
{
    public enum ListingCategory
    {
        [Description("Breeding Stock")] Breeding = 0,
        [Description("Antler / Velvet")] Antler = 1,
        [Description("Meat")] Meat = 2,
        [Description("Egg")] Egg = 3,
        [Description("Other")] Other = 99
    }

    public enum ListingStatus
    {
        [Description("Draft")] Draft = 0,
        [Description("Active")] Active = 1,
        [Description("Sold")] Sold = 2,
        [Description("Closed")] Closed = 3
    }

    public enum FarmVerificationStatus
    {
        [Description("Pending")] Pending = 0,
        [Description("Verified")] Verified = 1,
        [Description("Rejected")] Rejected = 2
    }
}
