using LostPeople.Application.Common.Interfaces;
using LostPeople.Domain.Entities;
using LostPeople.Infrastructure.Persistence;

namespace LostPeople.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly LostPeopleDbContext _context;

    public AuditService(LostPeopleDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(int? usuarioId, string entidad, int? entidadId, string tipoOperacion,
        string? valorAnterior, string? valorNuevo, string? ipOrigen = null,
        string? userAgent = null, string? detalles = null, CancellationToken ct = default)
    {
        _context.Auditorias.Add(new Auditoria
        {
            UsuarioId = usuarioId,
            Entidad = entidad,
            EntidadId = entidadId,
            TipoOperacion = tipoOperacion,
            ValorAnterior = valorAnterior,
            ValorNuevo = valorNuevo,
            IpOrigen = ipOrigen,
            UserAgent = userAgent,
            Detalles = detalles,
            Fecha = DateTime.UtcNow
        });
        await _context.SaveChangesAsync(ct);
    }
}
