using MediatR;

namespace LostPeople.Application.PersonasReportadas.Queries;

public class SearchPersonasQuery : IRequest<SearchPersonasResult>
{
    public string? Nombre { get; set; }
    public int? EdadDesde { get; set; }
    public int? EdadHasta { get; set; }
    public int? ProvinciaId { get; set; }
    public string? Estado { get; set; }
    public string? Sexo { get; set; }
    public string? TipoAlerta { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool IncluirSinteticos { get; set; }
}

public class SearchPersonasResult
{
    public List<PersonaReportadaItem> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(Total / (double)PageSize);
}

public class PersonaReportadaItem
{
    public int Id { get; set; }
    public string PrimerNombre { get; set; } = string.Empty;
    public string? SegundoNombre { get; set; }
    public string PrimerApellido { get; set; } = string.Empty;
    public string? SegundoApellido { get; set; }
    public int? EdadAproximada { get; set; }
    public string? Sexo { get; set; }
    public string? UltimaUbicacionTexto { get; set; }
    public string? TipoAlerta { get; set; }
    public string? EstadoCasoNombre { get; set; }
    public string? EstadoCasoColor { get; set; }
    public string? FotoThumbnailUrl { get; set; }
    public DateTime FechaDesaparicion { get; set; }
    public DateTime FechaCreacion { get; set; }
}
