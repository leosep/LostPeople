using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class EstadoCaso : IEntity
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ColorHex { get; set; }
    public int OrdenFlujo { get; set; }

    public ICollection<PersonaReportada> Personas { get; set; } = new List<PersonaReportada>();
}
