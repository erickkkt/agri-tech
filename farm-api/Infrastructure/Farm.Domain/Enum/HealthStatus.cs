
using System.ComponentModel;

namespace Farm.Domain.Enum
{
    public enum HealthStatus
    {
        [Description("Unknow")]
        Unknow = 0,

        [Description("Healthy")]
        Healthy = 1,

        [Description("Weak")]
        Weak = 2,

        [Description("Sick")]
        Sick = 3,

        [Description("Dead")]
        Dead = 4
    }
}
