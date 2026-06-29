using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class CentroSalud : IEntity
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Tipo { get; set; }
    public int ZonaId { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? ContactoEmail { get; set; }
    public bool Activo { get; set; } = true;

    public ZonaGeografica Zona { get; set; } = null!;
}
