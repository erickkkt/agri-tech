//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();



using Farm.Api.Extensions;
using Farm.Api.Hubs;
using Farm.Api.Middleware;
using Farm.Business.Jobs;
using Farm.Domain.FarmDbContexts;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                     .AddEnvironmentVariables();

builder.Logging.AddDebug()
               .AddConsole();

ConfigureServices(builder.Configuration, builder.Environment);

var app = builder.Build();

Configure(builder.Configuration, builder.Environment);

app.Run();

void ConfigureServices(ConfigurationManager configuration, IWebHostEnvironment environment)
{
    builder.Services.AddOptions();
    builder.Services.AddLogging();

    builder.Services.AddApplicationInsightsTelemetry();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = configuration.GetValue<string>("IdentityServerAuthentication:Authority");
        options.Audience = configuration.GetValue<string>("IdentityServerAuthentication:ClientId");
    })
    .AddJwtBearer("AzureAD", options =>
    {
        options.Authority = "https://login.microsoftonline.com/87b5b952-2320-4722-ab5b-402c1fb13b7c/v2.0";
        options.Audience = configuration.GetValue<string>("IdentityServerAuthentication:ClientId");
    })
    .AddJwtBearer("AzureAD_B2C", options =>
    {
        options.Authority = configuration.GetValue<string>("IdentityServerAuthentication:Authority");
        options.Audience = configuration.GetValue<string>("IdentityServerAuthentication:ClientId");
    });


    builder.Services.AddOptions();
    builder.Services.AddLogging();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            builder => builder
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials()
               .WithOrigins(
                   configuration.GetValue<string>("Admin:BaseUrl"),
                   configuration.GetValue<string>("Admin:ApiBaseUrl"),
                   configuration.GetValue<string>("Admin:SilentRefreshUrl")
               )
           .WithExposedHeaders("Location")
    );
    });

    builder.Services.AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                        new HeaderApiVersionReader("x-api-version"),
                                        new MediaTypeApiVersionReader("x-api-version"));
    });

    if (!environment.IsProduction())
    {
        // Configure Swagger
        builder.Services.AddSwaggerGen(
            options =>
            {
                var openApiSecurityScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer",
                    Type = SecuritySchemeType.ApiKey
                };

                options.AddSecurityDefinition("Bearer", openApiSecurityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        openApiSecurityScheme,
                        new List<string>()
                    }
                });
            });
    }

    if (configuration.GetConnectionString("farmDb") == null)
    {
        var adminDbConnString = Environment.GetEnvironmentVariable("ConnectionStrings__farmDb");

        builder.Services.AddDbContext<FarmDbContext>(options =>
            options.UseSqlServer(adminDbConnString, sqlServerOption => sqlServerOption.CommandTimeout(configuration.GetValue<int>("CommandTimeout"))));
    }
    else
    {
        builder.Services.AddDbContext<FarmDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("farmDb"), sqlServerOption => sqlServerOption.CommandTimeout(configuration.GetValue<int>("CommandTimeout"))));
    }

    builder.Services.AddBaseSettings(configuration);
    builder.Services.AddRepositories();
    builder.Services.AddServices(configuration);
    builder.Services.AddAutoMapper(cfg => cfg.ShouldMapMethod = (m => false), typeof(FarmDbContext).Assembly, typeof(Program).Assembly);
    builder.Services.AddControllers();

    // SignalR for realtime alerts / camera notifications
    builder.Services.AddSignalR();

    // Hangfire - background scheduled jobs (vaccine reminder, weight drop, low stock, daily report)
    var hangfireConn = configuration.GetConnectionString("farmDb")
                       ?? Environment.GetEnvironmentVariable("ConnectionStrings__farmDb");
    builder.Services.AddHangfire(cfg =>
    {
        cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
           .UseSimpleAssemblyNameTypeSerializer()
           .UseRecommendedSerializerSettings()
           .UseSqlServerStorage(hangfireConn, new SqlServerStorageOptions
           {
               PrepareSchemaIfNecessary = true,
               SchemaName = "Hangfire",
               QueuePollInterval = TimeSpan.FromSeconds(15),
               UseRecommendedIsolationLevel = true,
               DisableGlobalLocks = true
           });
    });
    builder.Services.AddHangfireServer();
}

void Configure(ConfigurationManager configuration, IWebHostEnvironment environment)
{
    using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        serviceScope.ServiceProvider.GetService<FarmDbContext>().Database.Migrate();
    }

    app.AddSecurityHeaders();
    app.UseHsts();

    if (!environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    if (environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors("CorsPolicy");

    app.UseMiddleware<GlobalExceptionHandler>();
    app.UseAuthentication();
    app.UseAuthorization();

    // Hangfire dashboard (consider restricting via authorization filter in production)
    app.UseHangfireDashboard("/hangfire");

    // Schedule recurring jobs after the app has started so DI is available
    RecurringJob.AddOrUpdate<IVaccineReminderJob>("vaccine-reminder", j => j.RunAsync(), Cron.Daily(6));
    RecurringJob.AddOrUpdate<IWeightDropDetectorJob>("weight-drop", j => j.RunAsync(), Cron.Daily(22));
    RecurringJob.AddOrUpdate<IStagnantGrowthJob>("stagnant-growth", j => j.RunAsync(), Cron.Weekly(DayOfWeek.Sunday));
    RecurringJob.AddOrUpdate<IFeedLowStockJob>("feed-low-stock", j => j.RunAsync(), "0 */6 * * *");
    RecurringJob.AddOrUpdate<IDailyReportJob>("daily-report", j => j.RunAsync(), "30 23 * * *");

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapHub<NotificationHub>("/hubs/notifications");
    });
}
