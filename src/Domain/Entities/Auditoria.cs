using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class Auditoria : IEntity
{
    public int Id { get; set; }
    public int? UsuarioId { get; set; }
    public string? Entidad { get; set; }
    public int? EntidadId { get; set; }
    public string? TipoOperacion { get; set; }
    public string? ValorAnterior { get; set; }
    public string? ValorNuevo { get; set; }
    public string? IpOrigen { get; set; }
    public string? UserAgent { get; set; }
    public string? Detalles { get; set; }
    public DateTime Fecha { get; set; }

    public Usuario? Usuario { get; set; }
}
