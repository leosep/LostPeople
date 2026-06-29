using System.Security.Claims;
using LostPeople.Infrastructure.Persistence;
using LostPeople.Infrastructure.Services;
using LostPeople.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LostPeople.Web.Controllers;

public class AccountController : Controller
{
    private readonly LostPeopleDbContext _context;

    public AccountController(LostPeopleDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Email == model.Email && u.Activo);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Password, usuario.PasswordHash))
        {
            ModelState.AddModelError("", "Correo o contraseña incorrectos.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.NombreCompleto ?? usuario.Email),
            new(ClaimTypes.Email, usuario.Email ?? ""),
            new(ClaimTypes.Role, usuario.Rol?.Nombre ?? "Ciudadano")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        if (usuario.Rol?.Nombre is "Admin" or "SuperAdmin")
            return RedirectToAction("Index", "Admin");

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
        {
            ModelState.AddModelError("Email", "Este correo ya está registrado.");
            return View(model);
        }

        var usuario = new Domain.Entities.Usuario
        {
            NombreCompleto = model.NombreCompleto,
            Email = model.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Telefono = model.Telefono,
            RolId = 1,
            Activo = true,
            Verificado = false,
            FechaCreacion = DateTime.UtcNow,
            AceptoTerminos = true,
            AceptoConfidencialidad = true
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Registro exitoso. Inicia sesión para continuar.";
        return RedirectToAction("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
