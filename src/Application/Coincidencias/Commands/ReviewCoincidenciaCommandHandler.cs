using LostPeople.Application.Common.Interfaces;
using LostPeople.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LostPeople.Application.Coincidencias.Commands;

public class ReviewCoincidenciaCommandHandler : IRequestHandler<ReviewCoincidenciaCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public ReviewCoincidenciaCommandHandler(IApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(ReviewCoincidenciaCommand request, CancellationToken ct)
    {
        var coincidencia = await _context.Coincidencias
            .Include(c => c.PersonaReportada)
            .FirstOrDefaultAsync(c => c.Id == request.CoincidenciaId, ct);

        if (coincidencia == null) return false;

        coincidencia.Revisada = true;
        coincidencia.ResultadoRevision = request.Resultado;
        coincidencia.NotasRevision = request.Notas;
        coincidencia.RevisorUsuarioId = request.RevisorUsuarioId;
        coincidencia.FechaRevision = DateTime.UtcNow;

        if (request.Resultado == "Confirmado" || request.Resultado == "EnVerificacion")
        {
            var verificacion = new Verificacion
            {
                CoincidenciaId = coincidencia.Id,
                VerificadorUsuarioId = request.RevisorUsuarioId,
                TipoVerificacion = request.Resultado == "Confirmado" ? "Confirmacion" : "Investigacion",
                Resultado = request.Resultado,
                MetodoContacto = request.MetodoContacto,
                DetalleContacto = request.DetalleContacto,
                Notas = request.Notas,
                FechaVerificacion = DateTime.UtcNow
            };
            _context.Verificaciones.Add(verificacion);
        }

        if (request.Resultado == "Confirmado")
        {
            var estadoCoincidencia = await _context.EstadosCaso.FirstAsync(e => e.Codigo == "COINCIDENCIA", ct);
            coincidencia.PersonaReportada.EstadoCasoId = estadoCoincidencia.Id;
            coincidencia.PersonaReportada.FechaUltimaActualizacion = DateTime.UtcNow;

            var reportes = await _context.Reportes
                .Where(r => r.PersonaId == coincidencia.PersonaReportada.Id)
                .ToListAsync(ct);
            var usuarioIds = reportes.Select(r => r.ReportanteUsuarioId).Distinct();
            foreach (var uid in usuarioIds)
            {
                await _notificationService.NotifyCaseClosedAsync(uid, coincidencia.PersonaReportada.Id, "COINCIDENCIA", ct);
            }
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }
}
