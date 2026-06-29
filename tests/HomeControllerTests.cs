using FluentAssertions;
using LostPeople.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LostPeople.Tests;

public class HomeControllerTests
{
    private readonly LostPeopleDbContext _context;

    public HomeControllerTests()
    {
        var options = new DbContextOptionsBuilder<LostPeopleDbContext>()
            .UseInMemoryDatabase(databaseName: "HomeTestDb_" + Guid.NewGuid())
            .Options;
        _context = new LostPeopleDbContext(options);
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _context.EstadosCaso.Add(new Domain.Entities.EstadoCaso { Id = 1, Codigo = "ACTIVO", Nombre = "Activo" });
        _context.EstadosCaso.Add(new Domain.Entities.EstadoCaso { Id = 5, Codigo = "LOCALIZADO", Nombre = "Localizado" });
        _context.PersonasReportadas.Add(new Domain.Entities.PersonaReportada
        {
            EstadoCasoId = 1,
            FechaCreacion = DateTime.UtcNow,
            DatosSinteticos = false
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task Index_ReturnsViewWithStatistics()
    {
        var controller = new LostPeople.Web.Controllers.HomeController(_context);
        
        var result = await controller.Index();
        
        result.Should().NotBeNull();
    }

    [Fact]
    public void Sobre_ReturnsView()
    {
        var controller = new LostPeople.Web.Controllers.HomeController(_context);
        
        var result = controller.Sobre();
        
        result.Should().NotBeNull();
    }

    [Fact]
    public void Contacto_ReturnsView()
    {
        var controller = new LostPeople.Web.Controllers.HomeController(_context);
        
        var result = controller.Contacto();
        
        result.Should().NotBeNull();
    }

    [Fact]
    public void Privacy_ReturnsView()
    {
        var controller = new LostPeople.Web.Controllers.HomeController(_context);
        
        var result = controller.Privacy();
        
        result.Should().NotBeNull();
    }
}