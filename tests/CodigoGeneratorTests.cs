using System.Text.RegularExpressions;
using LostPeople.Infrastructure.Services;
using Xunit;

namespace LostPeople.Tests;

public class CodigoGeneratorTests
{
    [Fact]
    public void GenerarCodigoSeguimiento_ReturnsValidFormat()
    {
        var result = CodigoGenerator.GenerarCodigoSeguimiento();
        
        Assert.False(string.IsNullOrEmpty(result));
        Assert.Matches(new Regex(@"^LP-[A-Z0-9]{5}$"), result);
    }

    [Fact]
    public void GenerarCodigoSeguimiento_ReturnsUniqueCodes()
    {
        var codes = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            var code = CodigoGenerator.GenerarCodigoSeguimiento();
            Assert.True(codes.Add(code), $"Duplicate code generated: {code}");
        }
    }

    [Fact]
    public void GenerarCodigoSeguimiento_ExcludesAmbiguousCharacters()
    {
        var result = CodigoGenerator.GenerarCodigoSeguimiento();
        
        Assert.DoesNotContain("I", result);
        Assert.DoesNotContain("O", result);
        Assert.DoesNotContain("0", result);
        Assert.DoesNotContain("1", result);
    }

    [Fact]
    public void GenerarCodigoVerificacion_ReturnsSixDigits()
    {
        var result = CodigoGenerator.GenerarCodigoVerificacion();
        
        Assert.False(string.IsNullOrEmpty(result));
        Assert.Equal(6, result.Length);
        Assert.True(result.All(char.IsDigit));
    }

    [Fact]
    public void GenerarCodigoVerificacion_ReturnsValidRange()
    {
        for (int i = 0; i < 100; i++)
        {
            var result = CodigoGenerator.GenerarCodigoVerificacion();
            var numericValue = int.Parse(result);
            Assert.InRange(numericValue, 100000, 999999);
        }
    }

    private static void AssertMatches(string pattern, string value)
    {
        Assert.Matches(new Regex(pattern), value);
    }
}