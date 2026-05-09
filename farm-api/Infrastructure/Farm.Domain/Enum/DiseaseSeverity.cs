using System.ComponentModel;

namespace Farm.Domain.Enum
{
    public enum DiseaseSeverity
    {
        [Description("Mild")]
        Mild = 0,

        [Description("Moderate")]
        Moderate = 1,

        [Description("Severe")]
        Severe = 2,

        [Description("Critical")]
        Critical = 3
    }

    public enum DiseaseStatus
    {
        [Description("Active")]
        Active = 0,

        [Description("UnderTreatment")]
        UnderTreatment = 1,

        [Description("Recovered")]
        Recovered = 2,

        [Description("Fatal")]
        Fatal = 3
    }
}
