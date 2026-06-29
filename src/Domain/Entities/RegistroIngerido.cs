using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class RegistroIngerido : IEntity
{
    public int Id { get; set; }
    public int FuenteId { get; set; }
    public string? IdentificadorExterno { get; set; }
    public string? PrimerNombre { get; set; }
    public string? SegundoNombre { get; set; }
    public string? PrimerApellido { get; set; }
    public string? SegundoApellido { get; set; }
    public string? Sexo { get; set; }
    public int? EdadAproximada { get; set; }
    public string? DescripcionFisica { get; set; }
    public string? UbicacionTexto { get; set; }
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }
    public string? TelefonoContacto { get; set; }
    public string? InstitucionOrigen { get; set; }
    public string? UrlOrigen { get; set; }
    public string? HtmlCrudo { get; set; }
    public string? EstadoPaciente { get; set; }
    public DateTime? FechaRegistroFuente { get; set; }
    public DateTime FechaIngesta { get; set; }
    public string? HashContenido { get; set; }
    public bool CoincidenciaProcesada { get; set; }
    public int? ScoreMaximoCoincidencia { get; set; }

    public FuenteDatos Fuente { get; set; } = null!;
    public ICollection<Coincidencia> Coincidencias { get; set; } = new List<Coincidencia>();
}
