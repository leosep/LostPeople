using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class Usuario : IEntity, IAuditableEntity
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string? Cedula { get; set; }
    public int RolId { get; set; }
    public int? ZonaAsignadaId { get; set; }
    public bool Activo { get; set; } = true;
    public bool Verificado { get; set; }
    public string? Institucion { get; set; }
    public string? Cargo { get; set; }
    public string? TokenVerificacion { get; set; }
    public DateTime? FechaUltimoAcceso { get; set; }
    public bool AceptoTerminos { get; set; }
    public bool AceptoConfidencialidad { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaUltimaActualizacion { get; set; }

    public Rol Rol { get; set; } = null!;
    public ZonaGeografica? ZonaAsignada { get; set; }
    public ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
    public ICollection<Verificacion> Verificaciones { get; set; } = new List<Verificacion>();
    public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
    public ICollection<SesionUsuario> Sesiones { get; set; } = new List<SesionUsuario>();
    public ICollection<Archivo> ArchivosSubidos { get; set; } = new List<Archivo>();
}
