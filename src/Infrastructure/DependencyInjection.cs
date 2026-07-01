using LostPeople.Application.Common.Interfaces;
using LostPeople.Domain.Interfaces;
using LostPeople.Infrastructure.BackgroundJobs;
using LostPeople.Infrastructure.Matching;
using LostPeople.Infrastructure.Persistence;
using LostPeople.Infrastructure.Scraping;
using LostPeople.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace LostPeople.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connStr))
        {
            var server = Environment.GetEnvironmentVariable("LOSTPEOPLE_DB_SERVER") ?? configuration["ConnectionStrings:Server"] ?? "localhost";
            var database = Environment.GetEnvironmentVariable("LOSTPEOPLE_DB_NAME") ?? configuration["ConnectionStrings:Database"] ?? "LostPeople";
            var user = Environment.GetEnvironmentVariable("LOSTPEOPLE_DB_USER") ?? configuration["ConnectionStrings:User"] ?? "sa";
            var password = Environment.GetEnvironmentVariable("LOSTPEOPLE_DB_PASSWORD") ?? configuration["ConnectionStrings:Password"] ?? "";
            connStr = $"Server={server};Database={database};User Id={user};Password={password};TrustServerCertificate=True;Connection Timeout=30;";
        }

        services.AddDbContext<LostPeopleDbContext>(options =>
            options.UseSqlServer(
                connStr,
                b => b.MigrationsAssembly(typeof(LostPeopleDbContext).Assembly.GetName().Name)));

        services.AddScoped<Domain.Interfaces.IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IMatchingService, FuzzyMatchingService>();

        services.AddScoped<PoliciaNacionalConnector>();
        services.AddScoped<DatosGobDoConnector>();
        services.AddScoped<HospitalSimuladoConnector>();
        services.AddScoped<DataSourceConnectorFactory>();

        services.AddHttpClient<PoliciaNacionalConnector>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("User-Agent", "LostPeople/1.0");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml");
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("es-DO,es;q=0.9");
        });

        services.AddHttpClient<DatosGobDoConnector>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddQuartz(q =>
        {
            q.UsePersistentStore(store =>
            {
                store.UseSqlServer(connStr);
                store.UseNewtonsoftJsonSerializer();
                store.UseClustering();
                store.PerformSchemaValidation = true;
            });

            q.UseSimpleTypeLoader();
            q.UseDefaultThreadPool(tp => tp.MaxConcurrency = 10);

            var scrapingJobKey = new JobKey("ScrapingSchedulerJob");
            q.AddJob<ScrapingSchedulerJob>(opts => opts.WithIdentity(scrapingJobKey));
            q.AddTrigger(opts => opts
                .ForJob(scrapingJobKey)
                .WithIdentity("ScrapingSchedulerTrigger")
                .WithCronSchedule("0 */30 * * * ?"));

            var matchingJobKey = new JobKey("MatchingSchedulerJob");
            q.AddJob<MatchingSchedulerJob>(opts => opts.WithIdentity(matchingJobKey));
            q.AddTrigger(opts => opts
                .ForJob(matchingJobKey)
                .WithIdentity("MatchingSchedulerTrigger")
                .WithCronSchedule("0 */15 * * * ?"));

            var healthJobKey = new JobKey("SourceHealthCheckJob");
            q.AddJob<SourceHealthCheckJob>(opts => opts.WithIdentity(healthJobKey));
            q.AddTrigger(opts => opts
                .ForJob(healthJobKey)
                .WithIdentity("HealthCheckTrigger")
                .WithCronSchedule("0 0 */6 * * ?"));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }
}
