
using Farm.Api.Extensions;
using Farm.Api.Hubs;
using Farm.Api.Middleware;
using Farm.Business.Jobs;
using Farm.Domain.FarmDbContexts;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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

    // CORS - .NET 10 hardened WithOrigins to throw on null entries, so we collect, filter, and dedupe.
    // Origin sources (in priority order):
    //   1. Cors:AllowedOrigins (string array)        ← preferred, configure in appsettings
    //   2. legacy Admin:BaseUrl / ApiBaseUrl / SilentRefreshUrl
    //   3. legacy Webuser:BaseUrl / ApiBaseUrl / SilentRefreshUrl   (used in appsettings.Azure.json)
    var corsOrigins = (configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
        .Concat(new[]
        {
            configuration.GetValue<string>("Admin:BaseUrl"),
            configuration.GetValue<string>("Admin:ApiBaseUrl"),
            configuration.GetValue<string>("Admin:SilentRefreshUrl"),
            configuration.GetValue<string>("Webuser:BaseUrl"),
            configuration.GetValue<string>("Webuser:ApiBaseUrl"),
            configuration.GetValue<string>("Webuser:SilentRefreshUrl"),
        })
        .Where(o => !string.IsNullOrWhiteSpace(o))
        .Select(o => o!.TrimEnd('/'))
        .Distinct()
        .ToArray();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithExposedHeaders("Location");

            if (corsOrigins.Length > 0)
            {
                policy.WithOrigins(corsOrigins).AllowCredentials();
            }
            else
            {
                // No origins configured (typical first-deploy scenario). Allow any origin so the
                // app starts; you cannot combine AllowAnyOrigin with AllowCredentials per CORS spec.
                policy.AllowAnyOrigin();
            }
        });
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

    // Swagger - always register the generator service so `app.UseSwagger()` middleware
    // can resolve ISwaggerProvider regardless of environment. Whether to expose the UI
    // (i.e. call `app.UseSwagger()` / `UseSwaggerUI()`) is a runtime decision below.
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Farm API", Version = "v1" });

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
            { openApiSecurityScheme, new List<string>() }
        });
    });

    var farmDbConn = configuration.GetConnectionString("farmDb")
                     ?? Environment.GetEnvironmentVariable("ConnectionStrings__farmDb");

    builder.Services.AddDbContext<FarmDbContext>(options =>
        options.UseNpgsql(farmDbConn, npgsqlOption =>
            npgsqlOption.CommandTimeout(configuration.GetValue<int>("CommandTimeout"))));

    builder.Services.AddBaseSettings(configuration);
    builder.Services.AddRepositories();
    builder.Services.AddServices(configuration);
    builder.Services.AddAutoMapper(cfg => cfg.ShouldMapMethod = (m => false), typeof(FarmDbContext).Assembly, typeof(Program).Assembly);
    builder.Services.AddControllers(options =>
    {
        // <Nullable>enable</Nullable> is on project-wide. By default that makes every
        // non-nullable reference-type property on a [FromBody]/[FromQuery] DTO implicitly
        // REQUIRED, so any omitted optional field (description, province, search q, …)
        // gets rejected with 400 before the controller runs. We turn it off and rely on
        // explicit [Required] / [Range] / etc. attributes for validation instead.
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });

    // SignalR for realtime alerts / camera notifications
    builder.Services.AddSignalR();

    // Hangfire - background scheduled jobs (vaccine reminder, weight drop, low stock, daily report)
    // var hangfireConn = configuration.GetConnectionString("farmDb")
    //                    ?? Environment.GetEnvironmentVariable("ConnectionStrings__farmDb");
    // builder.Services.AddHangfire(cfg =>
    // {
    //     cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    //        .UseSimpleAssemblyNameTypeSerializer()
    //        .UseRecommendedSerializerSettings()
    //        .UsePostgreSqlStorage(opt => opt.UseNpgsqlConnection(hangfireConn),
    //            new PostgreSqlStorageOptions
    //            {
    //                SchemaName = "hangfire",
    //                PrepareSchemaIfNecessary = true,
    //                QueuePollInterval = TimeSpan.FromSeconds(15),
    //                InvisibilityTimeout = TimeSpan.FromMinutes(30)
    //            });
    // });
    // builder.Services.AddHangfireServer();
}

void Configure(ConfigurationManager configuration, IWebHostEnvironment environment)
{
    using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        serviceScope.ServiceProvider.GetService<FarmDbContext>().Database.Migrate();
    }

    app.AddSecurityHeaders();
    app.UseHsts();

    app.UseSwagger();
    app.UseSwaggerUI();
   
    if (environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors("CorsPolicy");

    app.UseMiddleware<GlobalExceptionHandler>();
    app.UseAuthentication();
    app.UseAuthorization();

    // Hangfire dashboard (consider restricting via authorization filter in production)
    // app.UseHangfireDashboard("/hangfire");

    // Schedule recurring jobs after the app has started so DI is available
    // RecurringJob.AddOrUpdate<IVaccineReminderJob>("vaccine-reminder", j => j.RunAsync(), Cron.Daily(6));
    // RecurringJob.AddOrUpdate<IWeightDropDetectorJob>("weight-drop", j => j.RunAsync(), Cron.Daily(22));
    // RecurringJob.AddOrUpdate<IStagnantGrowthJob>("stagnant-growth", j => j.RunAsync(), Cron.Weekly(DayOfWeek.Sunday));
    // RecurringJob.AddOrUpdate<IFeedLowStockJob>("feed-low-stock", j => j.RunAsync(), "0 */6 * * *");
    // RecurringJob.AddOrUpdate<IDailyReportJob>("daily-report", j => j.RunAsync(), "30 23 * * *");

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapHub<NotificationHub>("/hubs/notifications");
    });
}
