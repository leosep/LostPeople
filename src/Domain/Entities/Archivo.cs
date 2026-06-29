using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class Archivo : IEntity
{
    public int Id { get; set; }
    public int? PersonaId { get; set; }
    public int? ReporteId { get; set; }
    public string NombreOriginal { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public string? TipoContenido { get; set; }
    public long TamanoBytes { get; set; }
    public string? Categoria { get; set; }
    public bool Publico { get; set; }
    public DateTime FechaSubida { get; set; }
    public int SubidoPorUsuarioId { get; set; }
    public Usuario? SubidoPorUsuario { get; set; }

    public PersonaReportada? Persona { get; set; }
    public Reporte? Reporte { get; set; }
}
