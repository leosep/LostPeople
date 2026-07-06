using MediatR;

namespace LostPeople.Application.PersonasReportadas.Commands;

public class CreateReportCommand : IRequest<int>
{
    public string PrimerNombre { get; set; } = string.Empty;
    public string? SegundoNombre { get; set; }
    public string PrimerApellido { get; set; } = string.Empty;
    public string? SegundoApellido { get; set; }
    public string? Alias { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public DateTime? FechaDesaparicion { get; set; }
    public int? EdadAproximada { get; set; }
    public string? Sexo { get; set; }
    public string? DescripcionFisica { get; set; }
    public decimal? EstaturaCm { get; set; }
    public string? ColorPiel { get; set; }
    public string? ColorOjos { get; set; }
    public string? ColorCabello { get; set; }
    public string? SenasParticulares { get; set; }
    public string? CondicionMedica { get; set; }
    public string? MedicamentosRequeridos { get; set; }
    public string? Vestimenta { get; set; }
    public string? UltimaUbicacionTexto { get; set; }
    public decimal? UltimaUbicacionLat { get; set; }
    public decimal? UltimaUbicacionLng { get; set; }
    public int? UltimaUbicacionZonaId { get; set; }
    public string? RelacionConDesaparecido { get; set; }
    public string? TelefonoContacto { get; set; }
    public string? EmailContacto { get; set; }
    public bool AceptoTerminos { get; set; }
    public bool AceptoConfidencialidad { get; set; }
}
