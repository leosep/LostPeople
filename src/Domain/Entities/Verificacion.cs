using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class Verificacion : IEntity
{
    public int Id { get; set; }
    public int CoincidenciaId { get; set; }
    public int VerificadorUsuarioId { get; set; }
    public string? TipoVerificacion { get; set; }
    public string? Resultado { get; set; }
    public string? MetodoContacto { get; set; }
    public string? DetalleContacto { get; set; }
    public string? Notas { get; set; }
    public DateTime FechaVerificacion { get; set; }

    public Coincidencia Coincidencia { get; set; } = null!;
    public Usuario Verificador { get; set; } = null!;
}
