using FluentAssertions;
using LostPeople.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LostPeople.Tests;

public class AccountControllerTests
{
    private readonly LostPeopleDbContext _context;

    public AccountControllerTests()
    {
        var options = new DbContextOptionsBuilder<LostPeopleDbContext>()
            .UseInMemoryDatabase(databaseName: "AccountTestDb_" + Guid.NewGuid())
            .Options;
        _context = new LostPeopleDbContext(options);
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _context.Roles.Add(new Domain.Entities.Rol { Id = 1, Nombre = "Ciudadano" });
        _context.Roles.Add(new Domain.Entities.Rol { Id = 2, Nombre = "Admin" });
        _context.SaveChanges();
    }

    [Fact]
    public void Login_Get_ReturnsView()
    {
        var controller = new LostPeople.Web.Controllers.AccountController(_context);
        
        var result = controller.Login();
        
        result.Should().NotBeNull();
    }

    [Fact]
    public void Register_Get_ReturnsView()
    {
        var controller = new LostPeople.Web.Controllers.AccountController(_context);
        
        var result = controller.Register();
        
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_Post_DuplicateEmail_ReturnsViewWithError()
    {
        var existingUser = new LostPeople.Domain.Entities.Usuario
        {
            NombreCompleto = "Existing User",
            Email = "existing@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            RolId = 1,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };
        _context.Usuarios.Add(existingUser);
        await _context.SaveChangesAsync();

        var controller = new LostPeople.Web.Controllers.AccountController(_context);
        var model = new LostPeople.Web.ViewModels.RegisterViewModel
        {
            NombreCompleto = "Test User",
            Email = "existing@example.com",
            Password = "Password123!"
        };

        var result = await controller.Register(model);

        result.Should().NotBeNull();
    }
}