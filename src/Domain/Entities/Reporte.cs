using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class Reporte : IEntity, IAuditableEntity
{
    public int Id { get; set; }
    public int PersonaId { get; set; }
    public int ReportanteUsuarioId { get; set; }
    public string? RelacionConDesaparecido { get; set; }
    public string? TelefonoContacto { get; set; }
    public string? EmailContacto { get; set; }
    public string? CodigoVerificacion { get; set; }
    public bool Verificado { get; set; }
    public string? Notas { get; set; }
    public string? FuenteReporte { get; set; }
    public string? IpOrigen { get; set; }
    public bool EsReporteLocalizacion { get; set; }
    public string? DetalleLocalizacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaUltimaActualizacion { get; set; }

    public PersonaReportada Persona { get; set; } = null!;
    public Usuario Reportante { get; set; } = null!;
    public ICollection<Archivo> Archivos { get; set; } = new List<Archivo>();
}
