using LostPeople.Application.Common.Interfaces;
using LostPeople.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LostPeople.Application.Coincidencias.Commands;

public class ReviewCoincidenciaCommandHandler : IRequestHandler<ReviewCoincidenciaCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ReviewCoincidenciaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
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
            coincidencia.PersonaReportada.EstadoCasoId = 3;
            coincidencia.PersonaReportada.FechaUltimaActualizacion = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }
}
