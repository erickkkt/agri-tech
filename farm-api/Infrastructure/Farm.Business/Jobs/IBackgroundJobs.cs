namespace Farm.Business.Jobs
{
    /// <summary>
    /// Marker interfaces so Hangfire can resolve concrete jobs via DI.
    /// Each job exposes RunAsync() which is enqueued / scheduled in Program.cs.
    /// </summary>
    public interface IVaccineReminderJob   { Task RunAsync(); }
    public interface IWeightDropDetectorJob { Task RunAsync(); }
    public interface IStagnantGrowthJob     { Task RunAsync(); }
    public interface IFeedLowStockJob       { Task RunAsync(); }
    public interface IDailyReportJob        { Task RunAsync(); }
}
