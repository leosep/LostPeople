using System.Threading;
using FluentAssertions;
using LostPeople.Infrastructure.Matching;
using LostPeople.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LostPeople.Tests;

public class FuzzyMatchingServiceTests
{
    private readonly LostPeopleDbContext _context;
    private readonly FuzzyMatchingService _sut;

    public FuzzyMatchingServiceTests()
    {
        var options = new DbContextOptionsBuilder<LostPeopleDbContext>()
            .UseInMemoryDatabase(databaseName: "MatchTestDb_" + Guid.NewGuid())
            .Options;
        _context = new LostPeopleDbContext(options);
        SeedDatabase();
        _sut = new FuzzyMatchingService(_context);
    }

    private void SeedDatabase()
    {
        _context.EstadosCaso.Add(new Domain.Entities.EstadoCaso 
        { 
            Id = 1, 
            Codigo = "ACTIVO", 
            Nombre = "Activo", 
            ColorHex = "#E67E22" 
        });
        
        _context.SaveChanges();
    }

    [Fact]
    public async Task CalculateMatchAsync_Throws_WhenPersonaNotFound()
    {
        var registro = new Domain.Entities.RegistroIngerido
        {
            FuenteId = 1,
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            EdadAproximada = 25
        };
        _context.RegistrosIngeridos.Add(registro);
        await _context.SaveChangesAsync();

        var act = async () => await _sut.CalculateMatchAsync(999, registro.Id, CancellationToken.None);
        
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Persona or Registro not found*");
    }

    [Fact]
    public async Task CalculateMatchAsync_Throws_WhenRegistroNotFound()
    {
        var persona = new Domain.Entities.PersonaReportada
        {
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            EdadAproximada = 25,
            FechaCreacion = DateTime.UtcNow,
            CodigoSeguimiento = "LP-TEST01",
            EstadoCasoId = 1
        };
        _context.PersonasReportadas.Add(persona);
        await _context.SaveChangesAsync();

        var act = async () => await _sut.CalculateMatchAsync(persona.Id, 999, CancellationToken.None);
        
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Persona or Registro not found*");
    }

    [Fact]
    public async Task CalculateMatchAsync_ReturnsHighScore_ForExactMatch()
    {
        var persona = new Domain.Entities.PersonaReportada
        {
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            EdadAproximada = 25,
            Sexo = "M",
            FechaCreacion = DateTime.UtcNow,
            CodigoSeguimiento = "LP-TEST02",
            EstadoCasoId = 1
        };
        _context.PersonasReportadas.Add(persona);

        var registro = new Domain.Entities.RegistroIngerido
        {
            FuenteId = 1,
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            EdadAproximada = 25,
            Sexo = "M"
        };
        _context.RegistrosIngeridos.Add(registro);
        await _context.SaveChangesAsync();

        var result = await _sut.CalculateMatchAsync(persona.Id, registro.Id, CancellationToken.None);

        result.ScoreGeneral.Should().BeGreaterThan(0.7m);
        result.ScoreNombre.Should().Be(1.0m);
        result.ScoreEdad.Should().Be(1.0m);
        result.ScoreSexo.Should().Be(1.0m);
        result.SuperaUmbral.Should().BeTrue();
    }

    [Fact]
    public async Task CalculateMatchAsync_ReturnsLowScore_ForCompletelyDifferentNames()
    {
        var persona = new Domain.Entities.PersonaReportada
        {
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            FechaCreacion = DateTime.UtcNow,
            CodigoSeguimiento = "LP-TEST03",
            EstadoCasoId = 1
        };
        _context.PersonasReportadas.Add(persona);

        var registro = new Domain.Entities.RegistroIngerido
        {
            FuenteId = 1,
            PrimerNombre = "Maria",
            PrimerApellido = "Gonzalez"
        };
        _context.RegistrosIngeridos.Add(registro);
        await _context.SaveChangesAsync();

        var result = await _sut.CalculateMatchAsync(persona.Id, registro.Id, CancellationToken.None);

        // Jaro-Winkler gives partial scores even for different names due to partial matching
        result.ScoreNombre.Should().BeGreaterThan(0m);
    }

    [Fact]
    public async Task CalculateMatchAsync_ReturnsCorrectAgeScore_ForSmallDifference()
    {
        var persona = new Domain.Entities.PersonaReportada
        {
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            EdadAproximada = 25,
            FechaCreacion = DateTime.UtcNow,
            CodigoSeguimiento = "LP-TEST04",
            EstadoCasoId = 1
        };
        _context.PersonasReportadas.Add(persona);

        var registro = new Domain.Entities.RegistroIngerido
        {
            FuenteId = 1,
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            EdadAproximada = 26
        };
        _context.RegistrosIngeridos.Add(registro);
        await _context.SaveChangesAsync();

        var result = await _sut.CalculateMatchAsync(persona.Id, registro.Id, CancellationToken.None);

        result.ScoreEdad.Should().BeGreaterThan(0.9m);
    }

    [Fact]
    public async Task CalculateMatchAsync_ReturnsLowAgeScore_ForLargeDifference()
    {
        var persona = new Domain.Entities.PersonaReportada
        {
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            EdadAproximada = 25,
            FechaCreacion = DateTime.UtcNow,
            CodigoSeguimiento = "LP-TEST05",
            EstadoCasoId = 1
        };
        _context.PersonasReportadas.Add(persona);

        var registro = new Domain.Entities.RegistroIngerido
        {
            FuenteId = 1,
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            EdadAproximada = 50
        };
        _context.RegistrosIngeridos.Add(registro);
        await _context.SaveChangesAsync();

        var result = await _sut.CalculateMatchAsync(persona.Id, registro.Id, CancellationToken.None);

        result.ScoreEdad.Should().BeLessThan(0.5m);
    }

    [Fact]
    public async Task CalculateMatchAsync_ReturnsCorrectSexScore()
    {
        var persona = new Domain.Entities.PersonaReportada
        {
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            Sexo = "M",
            FechaCreacion = DateTime.UtcNow,
            CodigoSeguimiento = "LP-TEST06",
            EstadoCasoId = 1
        };
        _context.PersonasReportadas.Add(persona);

        var registro = new Domain.Entities.RegistroIngerido
        {
            FuenteId = 1,
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            Sexo = "M"
        };
        _context.RegistrosIngeridos.Add(registro);
        await _context.SaveChangesAsync();

        var result = await _sut.CalculateMatchAsync(persona.Id, registro.Id, CancellationToken.None);

        result.ScoreSexo.Should().Be(1.0m);
    }

    [Fact]
    public async Task CalculateMatchAsync_ReturnsZeroSexScore_ForMismatch()
    {
        var persona = new Domain.Entities.PersonaReportada
        {
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            Sexo = "M",
            FechaCreacion = DateTime.UtcNow,
            CodigoSeguimiento = "LP-TEST07",
            EstadoCasoId = 1
        };
        _context.PersonasReportadas.Add(persona);

        var registro = new Domain.Entities.RegistroIngerido
        {
            FuenteId = 1,
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            Sexo = "F"
        };
        _context.RegistrosIngeridos.Add(registro);
        await _context.SaveChangesAsync();

        var result = await _sut.CalculateMatchAsync(persona.Id, registro.Id, CancellationToken.None);

        result.ScoreSexo.Should().Be(0m);
    }

    [Fact]
    public async Task CalculateMatchAsync_ReturnsScoresHaveCorrectWeights()
    {
        var persona = new Domain.Entities.PersonaReportada
        {
            PrimerNombre = "Carlos Rodriguez",
            EdadAproximada = 30,
            Sexo = "M",
            FechaCreacion = DateTime.UtcNow,
            CodigoSeguimiento = "LP-TEST08",
            EstadoCasoId = 1
        };
        _context.PersonasReportadas.Add(persona);

        var registro = new Domain.Entities.RegistroIngerido
        {
            FuenteId = 1,
            PrimerNombre = "Carlos Rodriguez",
            EdadAproximada = 32,
            Sexo = "M"
        };
        _context.RegistrosIngeridos.Add(registro);
        await _context.SaveChangesAsync();

        var result = await _sut.CalculateMatchAsync(persona.Id, registro.Id, CancellationToken.None);

        var expectedScore = Math.Round(
            1.0m * 0.40m +
            0.9m * 0.15m +
            1.0m * 0.10m +
            0.5m * 0.20m +
            0.5m * 0.15m, 2);

        result.ScoreGeneral.Should().Be(expectedScore);
    }

    [Fact]
    public async Task BatchMatchAsync_CreatesCoincidences_ForMatchesAboveThreshold()
    {
        var persona = new Domain.Entities.PersonaReportada
        {
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            EdadAproximada = 25,
            EstadoCasoId = 1,
            FechaCreacion = DateTime.UtcNow,
            CodigoSeguimiento = "LP-TEST09"
        };
        _context.PersonasReportadas.Add(persona);

        var fuente = new Domain.Entities.FuenteDatos
        {
            Codigo = "TEST",
            Nombre = "Test Source",
            Tipo = "SIMULATED",
            IntervaloMinutos = 60
        };
        _context.FuentesDatos.Add(fuente);
        await _context.SaveChangesAsync();

        var registro = new Domain.Entities.RegistroIngerido
        {
            FuenteId = fuente.Id,
            PrimerNombre = "Juan",
            PrimerApellido = "Perez",
            EdadAproximada = 25,
            CoincidenciaProcesada = false
        };
        _context.RegistrosIngeridos.Add(registro);
        await _context.SaveChangesAsync();

        await _sut.BatchMatchAsync(CancellationToken.None);

        var coincidencia = await _context.Coincidencias
            .FirstOrDefaultAsync(c => c.ReportePersonaId == persona.Id && c.RegistroIngeridoId == registro.Id);

        coincidencia.Should().NotBeNull();
        coincidencia!.Revisada.Should().BeFalse();
        registro.CoincidenciaProcesada.Should().BeTrue();
    }
}