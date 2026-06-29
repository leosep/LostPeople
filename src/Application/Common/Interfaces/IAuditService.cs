namespace LostPeople.Application.Common.Interfaces;

public interface IAuditService
{
    Task LogAsync(int? usuarioId, string entidad, int? entidadId, string tipoOperacion, string? valorAnterior, string? valorNuevo, string? ipOrigen = null, string? userAgent = null, string? detalles = null, CancellationToken ct = default);
}
