using LostPeople.Application.Coincidencias.Commands;
using LostPeople.Domain.Entities;
using LostPeople.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace LostPeople.Web.Controllers;

[Authorize(Roles = "Admin,SuperAdmin")]
[EnableRateLimiting("Public")]
public class AdminController : Controller
{
    private readonly LostPeopleDbContext _context;
    private readonly IMediator _mediator;

    public AdminController(LostPeopleDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
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
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new ReviewCoincidenciaCommand
        {
            CoincidenciaId = id,
            RevisorUsuarioId = userId,
            Resultado = estado,
            Notas = null
        });

        if (!result) return NotFound();

        TempData["Mensaje"] = "Coincidencia actualizada correctamente.";
        return RedirectToAction("Coincidencias");
    }
}
