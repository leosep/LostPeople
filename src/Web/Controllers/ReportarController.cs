using LostPeople.Application.Common.Interfaces;
using LostPeople.Domain.Entities;
using LostPeople.Domain.Interfaces;
using LostPeople.Infrastructure.Persistence;
using LostPeople.Infrastructure.Services;
using LostPeople.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LostPeople.Web.Controllers;

public class ReportarController : Controller
{
    private readonly LostPeopleDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ReportarController(LostPeopleDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.Provincias = await _context.ZonasGeograficas
            .Where(z => z.Tipo == "Provincia")
            .OrderBy(z => z.Nombre)
            .ToListAsync();
        return View(new ReportarViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ReportarViewModel model)
    {
        if (!model.AceptoTerminos)
            ModelState.AddModelError("AceptoTerminos", "Debes aceptar los términos y condiciones.");
        if (!model.AceptoConfidencialidad)
            ModelState.AddModelError("AceptoConfidencialidad", "Debes aceptar la política de confidencialidad.");

        if (!ModelState.IsValid)
        {
            ViewBag.Provincias = await _context.ZonasGeograficas
                .Where(z => z.Tipo == "Provincia")
                .OrderBy(z => z.Nombre)
                .ToListAsync();
            return View(model);
        }

        var persona = new PersonaReportada
        {
            PrimerNombre = model.PrimerNombre,
            SegundoNombre = model.SegundoNombre,
            PrimerApellido = model.PrimerApellido,
            SegundoApellido = model.SegundoApellido,
            Alias = model.Alias,
            FechaNacimiento = model.FechaNacimiento,
            FechaDesaparicion = model.FechaDesaparicion ?? DateTime.UtcNow,
            EdadAproximada = model.EdadAproximada,
            Sexo = model.Sexo,
            DescripcionFisica = model.DescripcionFisica,
            EstaturaCm = model.EstaturaCm,
            ColorPiel = model.ColorPiel,
            ColorOjos = model.ColorOjos,
            ColorCabello = model.ColorCabello,
            SenasParticulares = model.SenasParticulares,
            CondicionMedica = model.CondicionMedica,
            MedicamentosRequeridos = model.MedicamentosRequeridos,
            Vestimenta = model.Vestimenta,
            UltimaUbicacionTexto = model.UltimaUbicacionTexto,
            UltimaUbicacionLat = model.UltimaUbicacionLat,
            UltimaUbicacionLng = model.UltimaUbicacionLng,
            UltimaUbicacionZonaId = model.UltimaUbicacionZonaId,
            CodigoSeguimiento = CodigoGenerator.GenerarCodigoSeguimiento(),
            EstadoCasoId = 1,
            DatosSinteticos = false,
            FechaCreacion = DateTime.UtcNow
        };

        _context.PersonasReportadas.Add(persona);
        await _context.SaveChangesAsync();

        var usuarioAnonimo = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == "anonimo@lostpeople.do");
        if (usuarioAnonimo == null)
        {
            usuarioAnonimo = new Usuario
            {
                NombreCompleto = "Anónimo",
                Email = "anonimo@lostpeople.do",
                PasswordHash = Guid.NewGuid().ToString(),
                RolId = 1,
                Activo = true,
                FechaCreacion = DateTime.UtcNow,
                AceptoTerminos = model.AceptoTerminos,
                AceptoConfidencialidad = model.AceptoConfidencialidad
            };
            _context.Usuarios.Add(usuarioAnonimo);
            await _context.SaveChangesAsync();
        }

        if (model.FotoPersona != null && model.FotoPersona.Length > 0)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "fotos");
            Directory.CreateDirectory(uploadsDir);
            var ext = Path.GetExtension(model.FotoPersona.FileName);
            var fileName = $"{persona.CodigoSeguimiento}_{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.FotoPersona.CopyToAsync(stream);
            }
            persona.FotoOriginalUrl = $"/uploads/fotos/{fileName}";
            _context.PersonasReportadas.Update(persona);
            await _context.SaveChangesAsync();
        }

        var reporte = new Reporte
        {
            PersonaId = persona.Id,
            ReportanteUsuarioId = usuarioAnonimo.Id,
            RelacionConDesaparecido = model.RelacionConDesaparecido,
            TelefonoContacto = model.TelefonoContacto,
            EmailContacto = model.EmailContacto,
            CodigoVerificacion = CodigoGenerator.GenerarCodigoVerificacion(),
            Verificado = false,
            FuenteReporte = "Web",
            IpOrigen = HttpContext.Connection.RemoteIpAddress?.ToString(),
            FechaCreacion = DateTime.UtcNow
        };
        _context.Reportes.Add(reporte);
        await _context.SaveChangesAsync();

        TempData["CodigoSeguimiento"] = persona.CodigoSeguimiento;
        TempData["PersonaNombre"] = $"{persona.PrimerNombre} {persona.PrimerApellido}";

        if (!string.IsNullOrEmpty(reporte.TelefonoContacto))
        {
            var smsService = HttpContext.RequestServices.GetRequiredService<INotificationService>();
            await smsService.SendSmsAsync(reporte.TelefonoContacto,
                $"LostPeople RD: Tu código de verificación es {reporte.CodigoVerificacion}. " +
                $"Código de seguimiento: {persona.CodigoSeguimiento}. " +
                $"Guarda este código para dar seguimiento a tu caso.");
        }
        if (!string.IsNullOrEmpty(reporte.EmailContacto))
        {
            var emailService = HttpContext.RequestServices.GetRequiredService<INotificationService>();
            await emailService.SendEmailAsync(reporte.EmailContacto,
                "LostPeople RD - Confirmación de reporte",
                $"<h2>Reporte recibido</h2><p>Hola,</p><p>Hemos recibido tu reporte de desaparición de <strong>{persona.PrimerNombre} {persona.PrimerApellido}</strong>.</p>" +
                $"<p><strong>Código de seguimiento:</strong> {persona.CodigoSeguimiento}</p>" +
                $"<p><strong>Código de verificación:</strong> {reporte.CodigoVerificacion}</p>" +
                $"<p>Guarda estos códigos. Los necesitarás para dar seguimiento a tu caso.</p>" +
                $"<hr><p style='color:#666;font-size:12px;'>LostPeople RD - Plataforma complementaria. No sustituye al 911.</p>");
        }

        return RedirectToAction("Exito");
    }

    [HttpGet]
    public IActionResult Exito()
    {
        if (TempData["CodigoSeguimiento"] == null)
            return RedirectToAction("Index");
        return View();
    }

    [HttpGet]
    public IActionResult Localizada()
    {
        return View(new LocalizadaViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Localizada(LocalizadaViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        PersonaReportada? persona = null;

        if (!string.IsNullOrEmpty(model.CodigoSeguimiento))
        {
            persona = await _context.PersonasReportadas
                .FirstOrDefaultAsync(p => p.CodigoSeguimiento == model.CodigoSeguimiento);
        }
        else if (!string.IsNullOrEmpty(model.NombrePersona))
        {
            var term = model.NombrePersona.ToLower().Trim();
            persona = await _context.PersonasReportadas
                .Where(p => p.PrimerNombre.ToLower().Contains(term) || p.PrimerApellido.ToLower().Contains(term))
                .FirstOrDefaultAsync();
        }

        if (persona == null)
        {
            ModelState.AddModelError("", "No se encontró la persona. Verifica los datos.");
            return View(model);
        }

        persona.EstadoCasoId = model.EstaSalvo ? 5 : 6;
        persona.FechaUltimaActualizacion = DateTime.UtcNow;

        var anonUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == "anonimo@lostpeople.do");
        var reporteCierre = new Reporte
        {
            PersonaId = persona.Id,
            ReportanteUsuarioId = anonUser?.Id ?? 1,
            EsReporteLocalizacion = true,
            DetalleLocalizacion = model.LugarLocalizacion,
            FechaCreacion = DateTime.UtcNow
        };
        _context.Reportes.Add(reporteCierre);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Gracias por reportar. Un verificador confirmará la información.";
        return RedirectToAction("Exito");
    }

    [HttpGet]
    public IActionResult Seguimiento()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Seguimiento(string codigo)
    {
        if (string.IsNullOrEmpty(codigo))
        {
            ModelState.AddModelError("", "Ingresa el código de seguimiento.");
            return View();
        }

        var persona = await _context.PersonasReportadas
            .Include(p => p.EstadoCaso)
            .FirstOrDefaultAsync(p => p.CodigoSeguimiento == codigo);

        if (persona == null)
        {
            ModelState.AddModelError("", "Código no válido. Verifica e intenta de nuevo.");
            return View();
        }

        return RedirectToAction("Estado", new { codigo });
    }

    [HttpGet]
    public async Task<IActionResult> Estado(string codigo)
    {
        var persona = await _context.PersonasReportadas
            .Include(p => p.EstadoCaso)
            .Include(p => p.Reportes)
            .FirstOrDefaultAsync(p => p.CodigoSeguimiento == codigo);

        if (persona == null) return NotFound();
        return View(persona);
    }
}