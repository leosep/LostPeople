using LostPeople.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LostPeople.Web.Controllers;

public class HomeController : Controller
{
    private readonly LostPeopleDbContext _context;

    public HomeController(LostPeopleDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var casosActivos = await _context.PersonasReportadas
            .CountAsync(p => p.EstadoCasoId <= 4 && !p.DatosSinteticos);

        var casosResueltos = await _context.PersonasReportadas
            .CountAsync(p => p.EstadoCasoId >= 5 && !p.DatosSinteticos);

        var alertasActivas = await _context.PersonasReportadas
            .Where(p => p.EstadoCasoId <= 4 && !string.IsNullOrEmpty(p.TipoAlerta) && !p.DatosSinteticos)
            .Include(p => p.UltimaUbicacionZona)
            .Take(5)
            .ToListAsync();

        ViewBag.CasosActivos = casosActivos;
        ViewBag.CasosResueltos = casosResueltos;
        ViewBag.AlertasActivas = alertasActivas;

        return View();
    }

    public IActionResult Sobre()
    {
        return View();
    }

    public IActionResult Contacto()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult DataRetention()
    {
        return View();
    }

    public IActionResult Dsar()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
