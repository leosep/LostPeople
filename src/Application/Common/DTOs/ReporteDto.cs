namespace LostPeople.Application.Common.DTOs;

public class CreateReporteDto
{
    public string PrimerNombre { get; set; } = string.Empty;
    public string? SegundoNombre { get; set; }
    public string PrimerApellido { get; set; } = string.Empty;
    public string? SegundoApellido { get; set; }
    public string? Alias { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public DateTime FechaDesaparicion { get; set; }
    public int? EdadAproximada { get; set; }
    public string? Sexo { get; set; }
    public string? TipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? DescripcionFisica { get; set; }
    public decimal? EstaturaCm { get; set; }
    public string? ColorPiel { get; set; }
    public string? ColorOjos { get; set; }
    public string? ColorCabello { get; set; }
    public string? SenasParticulares { get; set; }
    public string? CondicionMedica { get; set; }
    public string? MedicamentosRequeridos { get; set; }
    public string? Vestimenta { get; set; }
    public string? UltimaUbicacionTexto { get; set; }
    public decimal? UltimaUbicacionLat { get; set; }
    public decimal? UltimaUbicacionLng { get; set; }
    public int? UltimaUbicacionZonaId { get; set; }
    public string? RelacionConDesaparecido { get; set; }
    public string? TelefonoContacto { get; set; }
    public string? EmailContacto { get; set; }
}

public class ReporteResultDto
{
    public int ReporteId { get; set; }
    public int PersonaId { get; set; }
    public string CodigoSeguimiento { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}

public class PersonaPublicDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public int? Edad { get; set; }
    public string? Sexo { get; set; }
    public string? Provincia { get; set; }
    public string? Estado { get; set; }
    public string? EstadoColor { get; set; }
    public string? FotoUrl { get; set; }
    public DateTime FechaDesaparicion { get; set; }
    public string? TipoAlerta { get; set; }
    public string? UltimaUbicacionTexto { get; set; }
}

public class BusquedaRequestDto
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
}

public class BusquedaResultDto
{
    public List<PersonaPublicDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
