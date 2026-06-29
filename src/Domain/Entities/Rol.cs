using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class Rol : IEntity
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Permisos { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
