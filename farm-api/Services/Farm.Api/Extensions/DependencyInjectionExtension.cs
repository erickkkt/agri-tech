using Farm.Api.Hubs;
using Farm.Api.Services;
using Farm.Business.Jobs;
using Farm.Business.Services;
using Farm.Business.Services.Interfaces;
using Farm.Domain.Repositories;
using Farm.Domain.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Farm.Api.Extensions
{
    /// <summary>
    /// Dependency Injection helper
    /// </summary>
    public static class DependencyInjectionExtension
    {
        public static void AddBaseSettings(this IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            // Base API Configuration Access
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            // Existing
            services.AddScoped<IAnimalRepository, AnimalRepository>();
            services.AddScoped<IFarmRepository, FarmRepository>();
            services.AddScoped<ICageRepository, CageRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            // Phase 1
            services.AddScoped<IVaccineRepository, VaccineRepository>();
            services.AddScoped<IVaccineScheduleRepository, VaccineScheduleRepository>();
            services.AddScoped<IDiseaseRecordRepository, DiseaseRecordRepository>();
            services.AddScoped<ITreatmentRepository, TreatmentRepository>();
            services.AddScoped<IFeedItemRepository, FeedItemRepository>();
            services.AddScoped<IFeedTransactionRepository, FeedTransactionRepository>();
            services.AddScoped<IFeedConsumptionRepository, FeedConsumptionRepository>();
            services.AddScoped<IGrowthLogRepository, GrowthLogRepository>();
            services.AddScoped<IAlertRepository, AlertRepository>();
        }

        public static void AddServices(this IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddHttpContextAccessor();

            // Existing
            services.AddScoped<IAnimalService, AnimalService>();
            services.AddScoped<IFarmService, FarmService>();
            services.AddScoped<ICageService, CageService>();
            services.AddScoped<IUserService, UserService>();

            // Phase 1
            services.AddScoped<IVaccineService, VaccineService>();
            services.AddScoped<IFeedService, FeedService>();
            services.AddScoped<IGrowthLogService, GrowthLogService>();
            services.AddScoped<IDiseaseService, DiseaseService>();
            services.AddScoped<IAlertService, AlertService>();
            services.AddScoped<IReportService, ReportService>();

            // Phase 2 - Marketplace
            services.AddScoped<IListingService, ListingService>();

            // Phase 3 - Investment
            services.AddScoped<IInvestmentService, InvestmentService>();

            // Hangfire jobs
            services.AddScoped<IVaccineReminderJob, VaccineReminderJob>();
            services.AddScoped<IWeightDropDetectorJob, WeightDropDetectorJob>();
            services.AddScoped<IStagnantGrowthJob, StagnantGrowthJob>();
            services.AddScoped<IFeedLowStockJob, FeedLowStockJob>();
            services.AddScoped<IDailyReportJob, DailyReportJob>();

            // Storage + Realtime
            services.AddScoped<IMediaStorageService, LocalFileStorageService>();
            services.AddScoped<INotificationPublisher, SignalRNotificationPublisher>();
        }

        public static void AddSecurityHeaders(this IApplicationBuilder app)
        {
            app.UseXfo(options => options.SameOrigin());
            app.UseHsts(options => options.MaxAge(days: 365).IncludeSubdomains());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXContentTypeOptions();
            app.UseCsp(options => options
                .DefaultSources(s => s.Self().CustomSources("data:").CustomSources("https:"))
                .StyleSources(s => s.CustomSources("*").UnsafeInline())
                .ScriptSources(s => s.Self().CustomSources("*").UnsafeInline())
                .FontSources(s => s.CustomSources("*"))
                .ImageSources(s => s.CustomSources("*"))
            );
            app.UseReferrerPolicy(opts => opts.StrictOrigin());
        }
    }
}
