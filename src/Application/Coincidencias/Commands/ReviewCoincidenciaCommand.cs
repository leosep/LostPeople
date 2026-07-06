using MediatR;

namespace LostPeople.Application.Coincidencias.Commands;

public class ReviewCoincidenciaCommand : IRequest<bool>
{
    public int CoincidenciaId { get; set; }
    public int RevisorUsuarioId { get; set; }
    public string Resultado { get; set; } = string.Empty;
    public string? Notas { get; set; }
    public string? MetodoContacto { get; set; }
    public string? DetalleContacto { get; set; }
}
