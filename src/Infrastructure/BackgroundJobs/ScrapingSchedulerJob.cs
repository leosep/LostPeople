using LostPeople.Application.Common.Interfaces;
using LostPeople.Infrastructure.Scraping;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LostPeople.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class ScrapingSchedulerJob : IJob
{
    private readonly DataSourceConnectorFactory _factory;
    private readonly ILogger<ScrapingSchedulerJob> _logger;

    public ScrapingSchedulerJob(DataSourceConnectorFactory factory, ILogger<ScrapingSchedulerJob> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Iniciando ejecución de scraping automatizado...");

        var connectors = _factory.GetAllConnectors();
        foreach (var connector in connectors)
        {
            try
            {
                _logger.LogInformation("Ejecutando conector: {SourceName}", connector.SourceName);
                var result = await connector.FetchAsync(context.CancellationToken);

                if (result.Success)
                    _logger.LogInformation("Conector {SourceName}: {Inserted} registros insertados, {Duration}ms",
                        connector.SourceName, result.RecordsInserted, result.Duration.TotalMilliseconds);
                else
                    _logger.LogWarning("Conector {SourceName} falló: {Errors}",
                        connector.SourceName, string.Join("; ", result.Errors.Select(e => e.Message)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en conector {SourceName}", connector.SourceName);
            }
        }

        _logger.LogInformation("Ejecución de scraping completada.");
    }
}
