using System.ComponentModel;

namespace Farm.Domain.Enum
{
    public enum FeedTransactionType
    {
        [Description("Stock In")]
        In = 0,

        [Description("Stock Out")]
        Out = 1,

        [Description("Adjustment")]
        Adjustment = 2,

        [Description("Loss")]
        Loss = 3
    }
}
