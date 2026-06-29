using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class SesionUsuario : IEntity
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public string? IpOrigen { get; set; }
    public string? UserAgent { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaExpiracion { get; set; }
    public bool Activa { get; set; } = true;

    public Usuario Usuario { get; set; } = null!;
}
