using System.ComponentModel;

namespace Farm.Domain.Enum
{
    public enum VaccineStatus
    {
        [Description("Scheduled")]
        Scheduled = 0,

        [Description("Administered")]
        Administered = 1,

        [Description("Missed")]
        Missed = 2,

        [Description("Cancelled")]
        Cancelled = 3
    }
}
