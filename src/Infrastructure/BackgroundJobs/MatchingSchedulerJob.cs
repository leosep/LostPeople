using LostPeople.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LostPeople.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class MatchingSchedulerJob : IJob
{
    private readonly IMatchingService _matchingService;
    private readonly ILogger<MatchingSchedulerJob> _logger;

    public MatchingSchedulerJob(IMatchingService matchingService, ILogger<MatchingSchedulerJob> logger)
    {
        _matchingService = matchingService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Iniciando ejecución del motor de matching...");

        try
        {
            var matches = await _matchingService.BatchMatchAsync(context.CancellationToken);
            _logger.LogInformation("Matching completado: {Count} nuevas coincidencias encontradas", matches.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ejecución de matching");
        }
    }
}
