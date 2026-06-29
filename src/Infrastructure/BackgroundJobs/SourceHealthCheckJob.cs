using LostPeople.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LostPeople.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class SourceHealthCheckJob : IJob
{
    private readonly LostPeopleDbContext _context;
    private readonly ILogger<SourceHealthCheckJob> _logger;

    public SourceHealthCheckJob(LostPeopleDbContext context, ILogger<SourceHealthCheckJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Verificando salud de fuentes de datos...");

        var fuentes = await _context.FuentesDatos.ToListAsync(context.CancellationToken);
        foreach (var fuente in fuentes)
        {
            if (fuente.FallosConsecutivos >= 3)
            {
                fuente.EstadoSalud = "Caída";
                _logger.LogWarning("Fuente {Nombre} marcada como caída ({Fallos} fallos consecutivos)",
                    fuente.Nombre, fuente.FallosConsecutivos);
            }
            else if (fuente.FallosConsecutivos > 0)
            {
                fuente.EstadoSalud = "Inestable";
            }
            else
            {
                fuente.EstadoSalud = "Activa";
            }
        }

        await _context.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Verificación de salud completada.");
    }
}
