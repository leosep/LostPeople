using MediatR;

namespace LostPeople.Application.Coincidencias.Queries;

public class GetCoincidenciasQuery : IRequest<List<CoincidenciaItem>>
{
    public string? Estado { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class CoincidenciaItem
{
    public int Id { get; set; }
    public int ReportePersonaId { get; set; }
    public string PersonaNombre { get; set; } = string.Empty;
    public string PersonaApellido { get; set; } = string.Empty;
    public string? PersonaCodigoSeguimiento { get; set; }
    public string? RegistroNombre { get; set; }
    public string? FuenteNombre { get; set; }
    public decimal ScoreGeneral { get; set; }
    public bool Revisada { get; set; }
    public string? ResultadoRevision { get; set; }
    public DateTime FechaDeteccion { get; set; }
    public DateTime? FechaRevision { get; set; }
    public string? RevisorNombre { get; set; }
}
