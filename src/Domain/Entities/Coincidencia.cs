using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class Coincidencia : IEntity
{
    public int Id { get; set; }
    public int ReportePersonaId { get; set; }
    public int RegistroIngeridoId { get; set; }
    public decimal ScoreGeneral { get; set; }
    public decimal ScoreNombre { get; set; }
    public decimal ScoreEdad { get; set; }
    public decimal ScoreSexo { get; set; }
    public decimal ScoreUbicacion { get; set; }
    public decimal ScoreDescripcion { get; set; }
    public string? AlgoritmoUsado { get; set; }
    public bool Revisada { get; set; }
    public int? RevisorUsuarioId { get; set; }
    public string? ResultadoRevision { get; set; }
    public string? NotasRevision { get; set; }
    public DateTime FechaDeteccion { get; set; }
    public DateTime? FechaRevision { get; set; }

    public PersonaReportada PersonaReportada { get; set; } = null!;
    public RegistroIngerido RegistroIngerido { get; set; } = null!;
    public Usuario? Revisor { get; set; }
    public ICollection<Verificacion> Verificaciones { get; set; } = new List<Verificacion>();
}
