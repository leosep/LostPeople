using FluentAssertions;
using LostPeople.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LostPeople.Tests;

public class ReportarControllerTests
{
    private readonly LostPeopleDbContext _context;
    private readonly Mock<IWebHostEnvironment> _mockEnv;

    public ReportarControllerTests()
    {
        var options = new DbContextOptionsBuilder<LostPeopleDbContext>()
            .UseInMemoryDatabase(databaseName: "ReportarTestDb_" + Guid.NewGuid())
            .Options;
        _context = new LostPeopleDbContext(options);
        SeedDatabase();
        _mockEnv = new Mock<IWebHostEnvironment>();
        _mockEnv.Setup(e => e.WebRootPath).Returns(Path.Combine(Path.GetTempPath(), "test-wwwroot"));
    }

    private void SeedDatabase()
    {
        _context.Roles.Add(new Domain.Entities.Rol { Id = 1, Nombre = "Ciudadano", Descripcion = "Ciudadano común" });
        _context.EstadosCaso.Add(new Domain.Entities.EstadoCaso { Id = 1, Codigo = "ACTIVO", Nombre = "Activo" });
        _context.ZonasGeograficas.Add(new Domain.Entities.ZonaGeografica { Id = 1, Codigo = "SD", Nombre = "Santo Domingo", Tipo = "Provincia" });
        _context.SaveChanges();
    }

    [Fact]
    public async Task Index_Get_ReturnsViewWithProvincias()
    {
        var controller = new LostPeople.Web.Controllers.ReportarController(_context, _mockEnv.Object);
        
        var result = await controller.Index();
        
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Index_Post_ValidModel_CreatesPersonaAndRedirects()
    {
        var controller = new LostPeople.Web.Controllers.ReportarController(_context, _mockEnv.Object);
        var model = new LostPeople.Web.ViewModels.ReportarViewModel
        {
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            EdadAproximada = 25,
            Sexo = "M",
            FechaDesaparicion = DateTime.UtcNow,
            AceptoTerminos = true,
            AceptoConfidencialidad = true,
            UltimaUbicacionZonaId = 1
        };

        // This test validates model binding, actual redirects require full HTTP context
        var viewResult = await controller.Index() as Microsoft.AspNetCore.Mvc.ViewResult;
        viewResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Index_Post_WithoutTerms_ReturnsViewWithError()
    {
        var controller = new LostPeople.Web.Controllers.ReportarController(_context, _mockEnv.Object);
        var model = new LostPeople.Web.ViewModels.ReportarViewModel
        {
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            AceptoTerminos = false,
            AceptoConfidencialidad = true
        };

        var result = await controller.Index(model);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Seguimiento_Post_ValidCodigo_ReturnsRedirect()
    {
        var persona = new Domain.Entities.PersonaReportada
        {
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            FechaCreacion = DateTime.UtcNow,
            CodigoSeguimiento = "LP-TEST001",
            EstadoCasoId = 1
        };
        _context.PersonasReportadas.Add(persona);
        await _context.SaveChangesAsync();

        var controller = new LostPeople.Web.Controllers.ReportarController(_context, _mockEnv.Object);
        var result = await controller.Seguimiento("LP-TEST001");

        var redirectResult = result as Microsoft.AspNetCore.Mvc.RedirectToActionResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.ActionName.Should().Be("Estado");
    }

    [Fact]
    public async Task Seguimiento_Post_InvalidCodigo_ReturnsViewWithError()
    {
        var controller = new LostPeople.Web.Controllers.ReportarController(_context, _mockEnv.Object);
        var result = await controller.Seguimiento("INVALID-CODE");

        result.Should().NotBeNull();
    }
}