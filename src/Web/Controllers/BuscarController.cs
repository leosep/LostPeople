using LostPeople.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace LostPeople.Web.Controllers;

[EnableRateLimiting("Public")]
public class BuscarController : Controller
{
    private readonly LostPeopleDbContext _context;

    public BuscarController(LostPeopleDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? nombre, int? edadDesde, int? edadHasta,
        int? provinciaId, string? estado, string? sexo, string? tipoAlerta,
        DateTime? fechaDesde, DateTime? fechaHasta, int page = 1)
    {
        if (page < 1) page = 1;
        var query = _context.PersonasReportadas
            .Where(p => !p.DatosSinteticos)
            .Include(p => p.EstadoCaso)
            .Include(p => p.UltimaUbicacionZona)
            .AsQueryable();

        if (!string.IsNullOrEmpty(nombre))
        {
            var term = nombre.ToLower().Trim();
            query = query.Where(p =>
                p.PrimerNombre.ToLower().Contains(term) ||
                (p.SegundoNombre != null && p.SegundoNombre.ToLower().Contains(term)) ||
                p.PrimerApellido.ToLower().Contains(term) ||
                (p.SegundoApellido != null && p.SegundoApellido.ToLower().Contains(term)));
        }

        if (edadDesde.HasValue)
            query = query.Where(p => p.EdadAproximada >= edadDesde.Value);
        if (edadHasta.HasValue)
            query = query.Where(p => p.EdadAproximada <= edadHasta.Value);
        if (provinciaId.HasValue)
            query = query.Where(p => p.UltimaUbicacionZonaId == provinciaId.Value);
        if (!string.IsNullOrEmpty(estado))
            query = query.Where(p => p.EstadoCaso.Codigo == estado);
        if (!string.IsNullOrEmpty(sexo))
            query = query.Where(p => p.Sexo != null && p.Sexo.ToLower() == sexo.ToLower());
        if (!string.IsNullOrEmpty(tipoAlerta))
            query = query.Where(p => p.TipoAlerta == tipoAlerta);
        if (fechaDesde.HasValue)
            query = query.Where(p => p.FechaDesaparicion >= fechaDesde.Value);
        if (fechaHasta.HasValue)
            query = query.Where(p => p.FechaDesaparicion <= fechaHasta.Value);

        var total = await query.CountAsync();
        var pageSize = 20;
        var personas = await query
            .OrderByDescending(p => p.FechaDesaparicion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Total = total;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

        ViewBag.Provincias = await _context.ZonasGeograficas
            .Where(z => z.Tipo == "Provincia")
            .OrderBy(z => z.Nombre)
            .ToListAsync();

        return View(personas);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var persona = await _context.PersonasReportadas
            .Include(p => p.EstadoCaso)
            .Include(p => p.UltimaUbicacionZona)
            .Include(p => p.Reportes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (persona == null) return NotFound();

        return View(persona);
    }
}
