using System.ComponentModel;

namespace Farm.Domain.Enum
{
    public enum InvestmentOfferStatus
    {
        [Description("Draft")] Draft = 0,
        [Description("Open")] Open = 1,
        [Description("Closed")] Closed = 2,
        [Description("Harvested")] Harvested = 3
    }

    public enum InvestmentOrderStatus
    {
        [Description("Pending")] Pending = 0,
        [Description("Confirmed")] Confirmed = 1,
        [Description("Cancelled")] Cancelled = 2,
        [Description("Refunded")] Refunded = 3
    }

    public enum HarvestType
    {
        [Description("Antler")] Antler = 0,
        [Description("Meat")] Meat = 1,
        [Description("Egg")] Egg = 2,
        [Description("Other")] Other = 99
    }

    public enum AnimalUpdateType
    {
        [Description("Feed")] Feed = 0,
        [Description("Vaccine")] Vaccine = 1,
        [Description("Photo")] Photo = 2,
        [Description("Weight")] Weight = 3,
        [Description("Health Check")] HealthCheck = 4,
        [Description("Note")] Note = 99
    }
}
