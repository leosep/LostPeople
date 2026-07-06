using LostPeople.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LostPeople.Application.Coincidencias.Queries;

public class GetCoincidenciasQueryHandler : IRequestHandler<GetCoincidenciasQuery, List<CoincidenciaItem>>
{
    private readonly IApplicationDbContext _context;

    public GetCoincidenciasQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CoincidenciaItem>> Handle(GetCoincidenciasQuery request, CancellationToken ct)
    {
        var query = _context.Coincidencias
            .Include(c => c.PersonaReportada)
            .Include(c => c.RegistroIngerido).ThenInclude(r => r.Fuente)
            .Include(c => c.Revisor)
            .AsQueryable();

        if (request.Estado == "Pendiente")
            query = query.Where(c => !c.Revisada);
        else if (!string.IsNullOrEmpty(request.Estado))
            query = query.Where(c => c.ResultadoRevision == request.Estado);

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        return await query
            .OrderByDescending(c => c.ScoreGeneral)
            .ThenByDescending(c => c.FechaDeteccion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CoincidenciaItem
            {
                Id = c.Id,
                ReportePersonaId = c.ReportePersonaId,
                PersonaNombre = c.PersonaReportada.PrimerNombre,
                PersonaApellido = c.PersonaReportada.PrimerApellido,
                PersonaCodigoSeguimiento = c.PersonaReportada.CodigoSeguimiento,
                RegistroNombre = c.RegistroIngerido.PrimerNombre,
                FuenteNombre = c.RegistroIngerido.Fuente.Nombre,
                ScoreGeneral = c.ScoreGeneral,
                Revisada = c.Revisada,
                ResultadoRevision = c.ResultadoRevision,
                FechaDeteccion = c.FechaDeteccion,
                FechaRevision = c.FechaRevision,
                RevisorNombre = c.Revisor != null ? c.Revisor.NombreCompleto : null
            })
            .ToListAsync(ct);
    }
}
