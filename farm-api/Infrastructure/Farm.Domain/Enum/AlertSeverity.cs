using System.ComponentModel;

namespace Farm.Domain.Enum
{
    public enum AlertSeverity
    {
        [Description("Info")]
        Info = 0,

        [Description("Warning")]
        Warning = 1,

        [Description("Critical")]
        Critical = 2
    }
}
