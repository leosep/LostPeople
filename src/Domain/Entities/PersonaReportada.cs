using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class PersonaReportada : IEntity, IAggregateRoot, IAuditableEntity
{
    public int Id { get; set; }
    public string PrimerNombre { get; set; } = string.Empty;
    public string? SegundoNombre { get; set; }
    public string PrimerApellido { get; set; } = string.Empty;
    public string? SegundoApellido { get; set; }
    public string? Alias { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public DateTime FechaDesaparicion { get; set; }
    public int? EdadAproximada { get; set; }
    public string? Sexo { get; set; }
    public string? TipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? Nacionalidad { get; set; }
    public string? DescripcionFisica { get; set; }
    public decimal? EstaturaCm { get; set; }
    public decimal? PesoKg { get; set; }
    public string? ColorPiel { get; set; }
    public string? ColorOjos { get; set; }
    public string? ColorCabello { get; set; }
    public string? SenasParticulares { get; set; }
    public string? CondicionMedica { get; set; }
    public string? MedicamentosRequeridos { get; set; }
    public string? Vestimenta { get; set; }
    public bool EsMenorEdad { get; set; }
    public bool EsDiscapacitado { get; set; }
    public bool EsAdultoMayor { get; set; }
    public bool EsViolenciaGenero { get; set; }
    public string? TipoAlerta { get; set; }
    public string? FotoThumbnailUrl { get; set; }
    public string? FotoWebUrl { get; set; }
    public string? FotoOriginalUrl { get; set; }
    public int EstadoCasoId { get; set; }
    public int? UltimaUbicacionZonaId { get; set; }
    public decimal? UltimaUbicacionLat { get; set; }
    public decimal? UltimaUbicacionLng { get; set; }
    public string? UltimaUbicacionTexto { get; set; }
    public string CodigoSeguimiento { get; set; } = string.Empty;
    public bool DatosSinteticos { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaUltimaActualizacion { get; set; }

    public EstadoCaso EstadoCaso { get; set; } = null!;
    public ZonaGeografica? UltimaUbicacionZona { get; set; }
    public ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
    public ICollection<Coincidencia> Coincidencias { get; set; } = new List<Coincidencia>();
    public ICollection<Archivo> Archivos { get; set; } = new List<Archivo>();
    public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
}
