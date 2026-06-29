using LostPeople.Application.Common.Interfaces;
using LostPeople.Domain.Entities;
using LostPeople.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LostPeople.Infrastructure.Matching;

public class FuzzyMatchingService : IMatchingService
{
    private readonly LostPeopleDbContext _context;

    public FuzzyMatchingService(LostPeopleDbContext context)
    {
        _context = context;
    }

    public async Task<MatchResult> CalculateMatchAsync(int personaId, int registroId, CancellationToken ct = default)
    {
        var persona = await _context.PersonasReportadas.FindAsync(new object[] { personaId }, ct);
        var registro = await _context.RegistrosIngeridos.FindAsync(new object[] { registroId }, ct);

        if (persona == null || registro == null)
            throw new ArgumentException("Persona or Registro not found");

        var nombreTokens = new[] {
            (persona.PrimerNombre ?? "").ToLower().Trim(),
            (persona.PrimerApellido ?? "").ToLower().Trim()
        };
        var regNombreTokens = new[] {
            (registro.PrimerNombre ?? "").ToLower().Trim(),
            (registro.PrimerApellido ?? "").ToLower().Trim()
        };

        var scoreNombre = CalculateJaroWinkler(
            string.Join(" ", nombreTokens.Where(n => !string.IsNullOrEmpty(n))),
            string.Join(" ", regNombreTokens.Where(n => !string.IsNullOrEmpty(n))));

        var diffEdad = Math.Abs((persona.EdadAproximada ?? 0) - (registro.EdadAproximada ?? 0));
        var scoreEdad = diffEdad <= 20 ? 1m - (diffEdad / 20m) : 0m;
        if (persona.EdadAproximada == null || registro.EdadAproximada == null)
            scoreEdad = 0.5m;

        var scoreSexo = string.IsNullOrEmpty(persona.Sexo) || string.IsNullOrEmpty(registro.Sexo)
            ? 0.5m
            : string.Equals(persona.Sexo, registro.Sexo, StringComparison.OrdinalIgnoreCase)
                ? 1m : 0m;

        var scoreUbicacion = 0.5m;
        if (!string.IsNullOrEmpty(persona.UltimaUbicacionTexto) && !string.IsNullOrEmpty(registro.UbicacionTexto))
        {
            var tokensPersona = persona.UltimaUbicacionTexto.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var tokensRegistro = registro.UbicacionTexto.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var comunes = tokensPersona.Intersect(tokensRegistro).Count();
            scoreUbicacion = tokensPersona.Length > 0
                ? (decimal)Math.Round((double)comunes / Math.Max(tokensPersona.Length, tokensRegistro.Length), 2)
                : 0.5m;
        }

        var scoreDescripcion = 0.5m;
        if (!string.IsNullOrEmpty(persona.DescripcionFisica) && !string.IsNullOrEmpty(registro.DescripcionFisica))
        {
            var tokensP = persona.DescripcionFisica.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var tokensR = registro.DescripcionFisica.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var comunes = tokensP.Intersect(tokensR).Count();
            scoreDescripcion = tokensP.Length > 0
                ? (decimal)Math.Round((double)comunes / Math.Max(tokensP.Length, tokensR.Length), 2)
                : 0.5m;
        }

        var scoreGeneral = Math.Round(
            scoreNombre * 0.40m +
            scoreEdad * 0.15m +
            scoreSexo * 0.10m +
            scoreUbicacion * 0.20m +
            scoreDescripcion * 0.15m, 2);

        return new MatchResult
        {
            PersonaId = personaId,
            RegistroId = registroId,
            ScoreGeneral = scoreGeneral,
            ScoreNombre = scoreNombre,
            ScoreEdad = scoreEdad,
            ScoreSexo = scoreSexo,
            ScoreUbicacion = scoreUbicacion,
            ScoreDescripcion = scoreDescripcion,
            AlgoritmoUsado = "WeightedJaroWinkler"
        };
    }

    public async Task<IReadOnlyList<MatchResult>> BatchMatchAsync(CancellationToken ct = default)
    {
        var resultados = new List<MatchResult>();

        var registrosSinProcesar = await _context.RegistrosIngeridos
            .Where(r => !r.CoincidenciaProcesada)
            .Take(500)
            .ToListAsync(ct);

        var personasActivas = await _context.PersonasReportadas
            .Where(p => p.EstadoCasoId <= 4)
            .ToListAsync(ct);

        foreach (var registro in registrosSinProcesar)
        {
            foreach (var persona in personasActivas)
            {
                var match = await CalculateMatchAsync(persona.Id, registro.Id, ct);
                if (match.SuperaUmbral)
                {
                    resultados.Add(match);
                    var coincidencia = new Coincidencia
                    {
                        ReportePersonaId = persona.Id,
                        RegistroIngeridoId = registro.Id,
                        ScoreGeneral = match.ScoreGeneral,
                        ScoreNombre = match.ScoreNombre,
                        ScoreEdad = match.ScoreEdad,
                        ScoreSexo = match.ScoreSexo,
                        ScoreUbicacion = match.ScoreUbicacion,
                        ScoreDescripcion = match.ScoreDescripcion,
                        AlgoritmoUsado = match.AlgoritmoUsado,
                        FechaDeteccion = DateTime.UtcNow
                    };
                    _context.Coincidencias.Add(coincidencia);
                }
            }
            registro.CoincidenciaProcesada = true;
        }

        await _context.SaveChangesAsync(ct);
        return resultados;
    }

    private static decimal CalculateJaroWinkler(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
            return 0m;

        var dist = (int)Math.Floor(Math.Max(s1.Length, s2.Length) / 2m) - 1;
        var matches1 = new bool[s1.Length];
        var matches2 = new bool[s2.Length];
        var matches = 0;
        var transpositions = 0;

        for (int i = 0; i < s1.Length; i++)
        {
            var start = Math.Max(0, i - dist);
            var end = Math.Min(i + dist + 1, s2.Length);
            for (int j = start; j < end; j++)
            {
                if (matches2[j] || s1[i] != s2[j]) continue;
                matches1[i] = true;
                matches2[j] = true;
                matches++;
                break;
            }
        }

        if (matches == 0) return 0m;

        var k = 0;
        for (int i = 0; i < s1.Length; i++)
        {
            if (!matches1[i]) continue;
            while (!matches2[k]) k++;
            if (s1[i] != s2[k]) transpositions++;
            k++;
        }

        var jaro = (matches / (double)s1.Length + matches / (double)s2.Length + (matches - transpositions / 2.0) / matches) / 3.0;
        var prefix = 0;
        for (int i = 0; i < Math.Min(4, Math.Min(s1.Length, s2.Length)); i++)
        {
            if (s1[i] == s2[i]) prefix++;
            else break;
        }

        return (decimal)Math.Round(jaro + prefix * 0.1 * (1 - jaro), 2);
    }
}
