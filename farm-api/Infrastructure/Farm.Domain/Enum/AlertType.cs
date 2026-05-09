using System.ComponentModel;

namespace Farm.Domain.Enum
{
    public enum AlertType
    {
        [Description("Missed Vaccine")]
        MissedVaccine = 0,

        [Description("Vaccine Reminder")]
        VaccineReminder = 1,

        [Description("Weight Drop")]
        WeightDrop = 2,

        [Description("Stagnant Growth")]
        StagnantGrowth = 3,

        [Description("Disease Detected")]
        DiseaseDetected = 4,

        [Description("Feed Low Stock")]
        FeedLowStock = 5,

        [Description("System")]
        System = 6
    }
}
