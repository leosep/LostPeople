using FluentAssertions;
using LostPeople.Infrastructure.Persistence;
using LostPeople.Infrastructure.Scraping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LostPeople.Tests;

public class HospitalSimuladoConnectorTests
{
    private readonly LostPeopleDbContext _context;
    private readonly Mock<ILogger<HospitalSimuladoConnector>> _mockLogger;

    public HospitalSimuladoConnectorTests()
    {
        var options = new DbContextOptionsBuilder<LostPeopleDbContext>()
            .UseInMemoryDatabase(databaseName: "HospitalSimuladoTestDb_" + Guid.NewGuid())
            .Options;
        _context = new LostPeopleDbContext(options);
        _mockLogger = new Mock<ILogger<HospitalSimuladoConnector>>();
    }

    [Fact]
    public void SourceCode_ReturnsCorrectValue()
    {
        var connector = new HospitalSimuladoConnector(_mockLogger.Object, _context);
        
        connector.SourceCode.Should().Be("HOSPITAL_SIMULADO");
    }

    [Fact]
    public void SourceName_ReturnsCorrectValue()
    {
        var connector = new HospitalSimuladoConnector(_mockLogger.Object, _context);
        
        connector.SourceName.Should().Be("Hospitales RD - Simulación de pacientes NN");
    }

    [Theory]
    [InlineData("SIMULATED")]
    [InlineData("SIMULADO")]
    [InlineData("HOSPITAL_SIMULADO")]
    public void CanHandle_ReturnsTrue_ForValidTypes(string sourceType)
    {
        var connector = new HospitalSimuladoConnector(_mockLogger.Object, _context);
        
        connector.CanHandle(sourceType).Should().BeTrue();
    }

    [Theory]
    [InlineData("POLICE")]
    [InlineData("INVALID")]
    [InlineData("OTHER")]
    public void CanHandle_ReturnsFalse_ForInvalidTypes(string sourceType)
    {
        var connector = new HospitalSimuladoConnector(_mockLogger.Object, _context);
        
        connector.CanHandle(sourceType).Should().BeFalse();
    }

    [Fact]
    public async Task FetchAsync_ReturnsSuccessResult()
    {
        _context.ZonasGeograficas.Add(new Domain.Entities.ZonaGeografica
        {
            Id = 1,
            Codigo = "SD",
            Nombre = "Santo Domingo",
            Tipo = "Provincia"
        });
        _context.SaveChanges();

        var connector = new HospitalSimuladoConnector(_mockLogger.Object, _context);
        
        var result = await connector.FetchAsync(CancellationToken.None);
        
        result.Success.Should().BeTrue();
        result.RecordsExtracted.Should().Be(15);
        result.RecordsInserted.Should().Be(15);
        result.Duration.Should().BeGreaterThan(TimeSpan.Zero);
    }
}