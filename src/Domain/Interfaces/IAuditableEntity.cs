namespace LostPeople.Domain.Interfaces;

public interface IAuditableEntity
{
    DateTime FechaCreacion { get; set; }
    DateTime? FechaUltimaActualizacion { get; set; }
}
