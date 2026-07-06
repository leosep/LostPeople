using LostPeople.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LostPeople.Application.PersonasReportadas.Queries;

public class SearchPersonasQueryHandler : IRequestHandler<SearchPersonasQuery, SearchPersonasResult>
{
    private readonly IApplicationDbContext _context;

    public SearchPersonasQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SearchPersonasResult> Handle(SearchPersonasQuery request, CancellationToken ct)
    {
        var query = _context.PersonasReportadas.AsQueryable();

        if (!request.IncluirSinteticos)
            query = query.Where(p => !p.DatosSinteticos);

        if (!string.IsNullOrEmpty(request.Nombre))
        {
            var term = request.Nombre.ToLower().Trim();
            query = query.Where(p =>
                p.PrimerNombre.ToLower().Contains(term) ||
                (p.SegundoNombre != null && p.SegundoNombre.ToLower().Contains(term)) ||
                p.PrimerApellido.ToLower().Contains(term) ||
                (p.SegundoApellido != null && p.SegundoApellido.ToLower().Contains(term)));
        }

        if (request.EdadDesde.HasValue)
            query = query.Where(p => p.EdadAproximada >= request.EdadDesde.Value);
        if (request.EdadHasta.HasValue)
            query = query.Where(p => p.EdadAproximada <= request.EdadHasta.Value);
        if (request.ProvinciaId.HasValue)
            query = query.Where(p => p.UltimaUbicacionZonaId == request.ProvinciaId.Value);
        if (!string.IsNullOrEmpty(request.Estado))
            query = query.Where(p => p.EstadoCaso!.Codigo == request.Estado);
        if (!string.IsNullOrEmpty(request.Sexo))
            query = query.Where(p => p.Sexo != null && p.Sexo.ToLower() == request.Sexo.ToLower());
        if (!string.IsNullOrEmpty(request.TipoAlerta))
            query = query.Where(p => p.TipoAlerta == request.TipoAlerta);
        if (request.FechaDesde.HasValue)
            query = query.Where(p => p.FechaDesaparicion >= request.FechaDesde.Value);
        if (request.FechaHasta.HasValue)
            query = query.Where(p => p.FechaDesaparicion <= request.FechaHasta.Value);

        var total = await query.CountAsync(ct);

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await query
            .OrderByDescending(p => p.FechaDesaparicion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PersonaReportadaItem
            {
                Id = p.Id,
                PrimerNombre = p.PrimerNombre,
                SegundoNombre = p.SegundoNombre,
                PrimerApellido = p.PrimerApellido,
                SegundoApellido = p.SegundoApellido,
                EdadAproximada = p.EdadAproximada,
                Sexo = p.Sexo,
                UltimaUbicacionTexto = p.UltimaUbicacionTexto,
                TipoAlerta = p.TipoAlerta,
                CodigoSeguimiento = p.CodigoSeguimiento,
                EstadoCasoNombre = p.EstadoCaso!.Nombre,
                EstadoCasoColor = p.EstadoCaso.ColorHex,
                FotoThumbnailUrl = p.FotoThumbnailUrl,
                FechaDesaparicion = p.FechaDesaparicion,
                FechaCreacion = p.FechaCreacion
            })
            .ToListAsync(ct);

        return new SearchPersonasResult
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }
}
