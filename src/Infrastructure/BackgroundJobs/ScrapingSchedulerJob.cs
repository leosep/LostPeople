using LostPeople.Application.Common.Interfaces;
using LostPeople.Domain.Entities;
using LostPeople.Infrastructure.Persistence;
using LostPeople.Infrastructure.Scraping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LostPeople.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class ScrapingSchedulerJob : IJob
{
    private readonly DataSourceConnectorFactory _factory;
    private readonly LostPeopleDbContext _context;
    private readonly ILogger<ScrapingSchedulerJob> _logger;

    public ScrapingSchedulerJob(DataSourceConnectorFactory factory, LostPeopleDbContext context, ILogger<ScrapingSchedulerJob> logger)
    {
        _factory = factory;
        _context = context;
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

                var fuente = await _context.FuentesDatos.FirstOrDefaultAsync(f => f.Codigo == connector.SourceCode, context.CancellationToken);
                if (fuente != null)
                {
                    fuente.TotalEjecuciones++;
                    fuente.UltimaEjecucionOk = result.Success ? DateTime.UtcNow : null;
                    fuente.TotalRegistrosObtenidos += result.RecordsInserted;

                    if (result.Success)
                    {
                        fuente.FallosConsecutivos = 0;
                        fuente.EstadoSalud = "Activa";
                    }
                    else
                    {
                        fuente.FallosConsecutivos++;
                        fuente.EstadoSalud = fuente.FallosConsecutivos >= 3 ? "Caída" : "Inestable";
                        fuente.UltimoError = DateTime.UtcNow;
                    }
                }

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

                var fuente = await _context.FuentesDatos.FirstOrDefaultAsync(f => f.Codigo == connector.SourceCode, context.CancellationToken);
                if (fuente != null)
                {
                    fuente.FallosConsecutivos++;
                    fuente.UltimoError = DateTime.UtcNow;
                    fuente.EstadoSalud = fuente.FallosConsecutivos >= 3 ? "Caída" : "Inestable";
                }
            }
        }

        await _context.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Ejecución de scraping completada.");
    }
}
