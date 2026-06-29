using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class Notificacion : IEntity
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int? ReportePersonaId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string? Titulo { get; set; }
    public string? Mensaje { get; set; }
    public bool Leida { get; set; }
    public bool Enviada { get; set; }
    public string? CanalEnvio { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaLectura { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public PersonaReportada? PersonaReportada { get; set; }
}
