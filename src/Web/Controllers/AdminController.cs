using LostPeople.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LostPeople.Web.Controllers;

[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminController : Controller
{
    private readonly LostPeopleDbContext _context;

    public AdminController(LostPeopleDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalPersonas = await _context.PersonasReportadas.CountAsync();
        ViewBag.TotalActivos = await _context.PersonasReportadas.CountAsync(p => p.EstadoCasoId <= 4);
        ViewBag.TotalLocalizados = await _context.PersonasReportadas.CountAsync(p => p.EstadoCasoId >= 5);
        ViewBag.TotalFuentes = await _context.FuentesDatos.CountAsync();
        ViewBag.TotalUsuarios = await _context.Usuarios.CountAsync();
        ViewBag.TotalCoincidencias = await _context.Coincidencias.CountAsync();
        ViewBag.CoincidenciasPendientes = await _context.Coincidencias.CountAsync(c => !c.Revisada);

        var ultimosReportes = await _context.Reportes
            .Include(r => r.Persona)
            .OrderByDescending(r => r.FechaCreacion)
            .Take(10)
            .ToListAsync();
        ViewBag.UltimosReportes = ultimosReportes;

        var fuentes = await _context.FuentesDatos.OrderBy(f => f.Nombre).ToListAsync();
        ViewBag.Fuentes = fuentes;

        return View();
    }

    public async Task<IActionResult> Fuentes()
    {
        var fuentes = await _context.FuentesDatos.OrderBy(f => f.Nombre).ToListAsync();
        return View(fuentes);
    }

    public async Task<IActionResult> Usuarios()
    {
        var usuarios = await _context.Usuarios.Include(u => u.Rol).OrderBy(u => u.FechaCreacion).ToListAsync();
        return View(usuarios);
    }

    public async Task<IActionResult> Coincidencias(string estado = "Pendiente")
    {
        var query = _context.Coincidencias
            .Include(c => c.PersonaReportada)
            .Include(c => c.RegistroIngerido)
            .AsQueryable();

        if (estado == "Pendiente")
            query = query.Where(c => !c.Revisada);
        else if (!string.IsNullOrEmpty(estado))
            query = query.Where(c => c.ResultadoRevision == estado);

        var coincidencias = await query.OrderByDescending(c => c.ScoreGeneral).ToListAsync();
        ViewBag.EstadoActual = estado;
        return View(coincidencias);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RevisarCoincidencia(int id, string estado)
    {
        var coincidencia = await _context.Coincidencias.FindAsync(id);
        if (coincidencia == null) return NotFound();

        coincidencia.Revisada = true;
        coincidencia.ResultadoRevision = estado;
        coincidencia.FechaRevision = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Coincidencia actualizada correctamente.";
        return RedirectToAction("Coincidencias");
    }
}
