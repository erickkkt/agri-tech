using Farm.Domain.ViewModels.Report;

namespace Farm.Business.Services.Interfaces
{
    public interface IReportService
    {
        Task<DashboardDto> GetDashboard(Guid? farmId = null);
    }
}
