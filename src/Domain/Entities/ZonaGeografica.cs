using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class ZonaGeografica : IEntity
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int? ZonaPadreId { get; set; }
    public decimal? LatitudCentroide { get; set; }
    public decimal? LongitudCentroide { get; set; }

    public ZonaGeografica? ZonaPadre { get; set; }
    public ICollection<ZonaGeografica> Hijos { get; set; } = new List<ZonaGeografica>();
    public ICollection<PersonaReportada> PersonasReportadas { get; set; } = new List<PersonaReportada>();
    public ICollection<Usuario> UsuariosAsignados { get; set; } = new List<Usuario>();
    public ICollection<CentroSalud> CentrosSalud { get; set; } = new List<CentroSalud>();
}
