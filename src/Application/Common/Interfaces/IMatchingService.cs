namespace LostPeople.Application.Common.Interfaces;

public interface IMatchingService
{
    Task<MatchResult> CalculateMatchAsync(int personaId, int registroId, CancellationToken ct = default);
    Task<IReadOnlyList<MatchResult>> BatchMatchAsync(CancellationToken ct = default);
}

public class MatchResult
{
    public int PersonaId { get; set; }
    public int RegistroId { get; set; }
    public decimal ScoreGeneral { get; set; }
    public decimal ScoreNombre { get; set; }
    public decimal ScoreEdad { get; set; }
    public decimal ScoreSexo { get; set; }
    public decimal ScoreUbicacion { get; set; }
    public decimal ScoreDescripcion { get; set; }
    public string AlgoritmoUsado { get; set; } = string.Empty;
    public bool SuperaUmbral => ScoreGeneral >= 0.5m;
}
