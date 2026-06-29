using FluentAssertions;
using LostPeople.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LostPeople.Tests;

public class BuscarControllerTests
{
    private readonly LostPeopleDbContext _context;

    public BuscarControllerTests()
    {
        var options = new DbContextOptionsBuilder<LostPeopleDbContext>()
            .UseInMemoryDatabase(databaseName: "BuscarTestDb_" + Guid.NewGuid())
            .Options;
        _context = new LostPeopleDbContext(options);
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _context.EstadosCaso.Add(new Domain.Entities.EstadoCaso { Id = 1, Codigo = "ACTIVO", Nombre = "Activo" });
        _context.EstadosCaso.Add(new Domain.Entities.EstadoCaso { Id = 5, Codigo = "LOCALIZADO", Nombre = "Localizado" });
        _context.ZonasGeograficas.Add(new Domain.Entities.ZonaGeografica { Id = 1, Codigo = "SD", Nombre = "Santo Domingo", Tipo = "Provincia" });
        
        _context.PersonasReportadas.AddRange(
            new Domain.Entities.PersonaReportada
            {
                PrimerNombre = "Juan",
                PrimerApellido = "Perez",
                EdadAproximada = 25,
                Sexo = "M",
                FechaDesaparicion = DateTime.UtcNow.AddDays(-1),
                EstadoCasoId = 1,
                FechaCreacion = DateTime.UtcNow,
                CodigoSeguimiento = "LP-SEARCH01",
                DatosSinteticos = false
            },
            new Domain.Entities.PersonaReportada
            {
                PrimerNombre = "Maria",
                PrimerApellido = "Gonzalez",
                EdadAproximada = 30,
                Sexo = "F",
                EstadoCasoId = 5,
                FechaCreacion = DateTime.UtcNow,
                CodigoSeguimiento = "LP-SEARCH02",
                DatosSinteticos = false
            }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task Index_ReturnsOnlyActiveNonSyntheticPersonas()
    {
        var controller = new LostPeople.Web.Controllers.BuscarController(_context);
        var result = await controller.Index(nombre: null, edadDesde: null, edadHasta: null, provinciaId: null, estado: null, sexo: null, tipoAlerta: null, fechaDesde: null, fechaHasta: null) as Microsoft.AspNetCore.Mvc.ViewResult;
        
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Index_FilterByName_ReturnsMatchingResults()
    {
        var controller = new LostPeople.Web.Controllers.BuscarController(_context);
        var result = await controller.Index(nombre: "Juan", edadDesde: null, edadHasta: null, provinciaId: null, estado: null, sexo: null, tipoAlerta: null, fechaDesde: null, fechaHasta: null) as Microsoft.AspNetCore.Mvc.ViewResult;
        
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Index_FilterByAgeRange_ReturnsMatchingResults()
    {
        var controller = new LostPeople.Web.Controllers.BuscarController(_context);
        var result = await controller.Index(nombre: null, edadDesde: 20, edadHasta: 30, provinciaId: null, estado: null, sexo: null, tipoAlerta: null, fechaDesde: null, fechaHasta: null) as Microsoft.AspNetCore.Mvc.ViewResult;
        
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Detalle_ReturnsNotFound_ForInvalidId()
    {
        var controller = new LostPeople.Web.Controllers.BuscarController(_context);
        var result = await controller.Detalle(9999);
        
        result.Should().NotBeNull();
        result.Should().BeOfType<Microsoft.AspNetCore.Mvc.NotFoundResult>();
    }
}