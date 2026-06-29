namespace LostPeople.Application.Common.DTOs;

public class MatchPendienteDto
{
    public int MatchId { get; set; }
    public int PersonaId { get; set; }
    public string NombrePersona { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public decimal ScoreGeneral { get; set; }
    public string FuenteOrigen { get; set; } = string.Empty;
    public string? InstitucionOrigen { get; set; }
    public DateTime FechaDeteccion { get; set; }
    public DateTime? FechaDesaparicion { get; set; }
    public string? UbicacionTexto { get; set; }
}

public class MatchDetalleDto
{
    public int MatchId { get; set; }
    public PersonaPublicDto Persona { get; set; } = null!;
    public RegistroDto Registro { get; set; } = null!;
    public decimal ScoreGeneral { get; set; }
    public decimal ScoreNombre { get; set; }
    public decimal ScoreEdad { get; set; }
    public decimal ScoreSexo { get; set; }
    public decimal ScoreUbicacion { get; set; }
    public decimal ScoreDescripcion { get; set; }
    public string AlgoritmoUsado { get; set; } = string.Empty;
}

public class RegistroDto
{
    public int Id { get; set; }
    public string? PrimerNombre { get; set; }
    public string? SegundoNombre { get; set; }
    public string? PrimerApellido { get; set; }
    public string? SegundoApellido { get; set; }
    public string? Sexo { get; set; }
    public int? EdadAproximada { get; set; }
    public string? DescripcionFisica { get; set; }
    public string? UbicacionTexto { get; set; }
    public string? InstitucionOrigen { get; set; }
    public string? FuenteNombre { get; set; }
    public DateTime? FechaRegistroFuente { get; set; }
}
