namespace Farm.Domain.ViewModels.Report
{
    public class DashboardDto
    {
        public int TotalFarms { get; set; }
        public int TotalCages { get; set; }
        public int TotalAnimals { get; set; }
        public int HealthyAnimals { get; set; }
        public int SickAnimals { get; set; }

        public int VaccineSchedulesUpcoming { get; set; }
        public int VaccineSchedulesOverdue { get; set; }

        public int FeedItemsLowStock { get; set; }

        public int UnreadAlerts { get; set; }
        public int CriticalAlerts { get; set; }

        public List<SpeciesCountDto> AnimalsBySpecies { get; set; } = new();
        public List<HealthCountDto> AnimalsByHealth { get; set; } = new();
        public List<MonthlyGrowthDto> AverageGrowthByMonth { get; set; } = new();
    }

    public class SpeciesCountDto
    {
        public string Species { get; set; }
        public int Count { get; set; }
    }

    public class HealthCountDto
    {
        public string HealthStatus { get; set; }
        public int Count { get; set; }
    }

    public class MonthlyGrowthDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public float AverageWeight { get; set; }
        public int LogsCount { get; set; }
    }
}
