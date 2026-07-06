using AutoMapper;
using LostPeople.Domain.Entities;

namespace LostPeople.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PersonaReportada, PersonaReportadaDto>();
        CreateMap<RegistroIngerido, RegistroIngeridoDto>();
        CreateMap<Reporte, ReporteDto>();
        CreateMap<Coincidencia, CoincidenciaDto>();
        CreateMap<FuenteDatos, FuenteDatosDto>();
        CreateMap<Usuario, UsuarioDto>();
    }
}

public class PersonaReportadaDto
{
    public int Id { get; set; }
    public string PrimerNombre { get; set; } = string.Empty;
    public string? SegundoNombre { get; set; }
    public string PrimerApellido { get; set; } = string.Empty;
    public string? SegundoApellido { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public DateTime FechaDesaparicion { get; set; }
    public int? EdadAproximada { get; set; }
    public string? Sexo { get; set; }
    public string? DescripcionFisica { get; set; }
    public string? Vestimenta { get; set; }
    public string? UltimaUbicacionTexto { get; set; }
    public string CodigoSeguimiento { get; set; } = string.Empty;
    public int EstadoCasoId { get; set; }
    public string? EstadoCasoNombre { get; set; }
    public string? FotoThumbnailUrl { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class RegistroIngeridoDto
{
    public int Id { get; set; }
    public int FuenteId { get; set; }
    public string? IdentificadorExterno { get; set; }
    public string? PrimerNombre { get; set; }
    public string? PrimerApellido { get; set; }
    public string? Sexo { get; set; }
    public int? EdadAproximada { get; set; }
    public string? DescripcionFisica { get; set; }
    public string? UbicacionTexto { get; set; }
    public string? InstitucionOrigen { get; set; }
    public DateTime FechaIngesta { get; set; }
    public string? FuenteNombre { get; set; }
}

public class ReporteDto
{
    public int Id { get; set; }
    public int PersonaId { get; set; }
    public string? RelacionConDesaparecido { get; set; }
    public string? TelefonoContacto { get; set; }
    public string? EmailContacto { get; set; }
    public string? CodigoVerificacion { get; set; }
    public bool Verificado { get; set; }
    public string? FuenteReporte { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class CoincidenciaDto
{
    public int Id { get; set; }
    public int ReportePersonaId { get; set; }
    public int RegistroIngeridoId { get; set; }
    public decimal ScoreGeneral { get; set; }
    public decimal ScoreNombre { get; set; }
    public decimal ScoreEdad { get; set; }
    public bool Revisada { get; set; }
    public string? ResultadoRevision { get; set; }
    public DateTime FechaDeteccion { get; set; }
    public string? PersonaNombre { get; set; }
    public string? RegistroNombre { get; set; }
}

public class FuenteDatosDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public string? EstadoSalud { get; set; }
    public DateTime? UltimaEjecucionOk { get; set; }
    public DateTime? UltimoError { get; set; }
    public int TotalEjecuciones { get; set; }
    public int TotalRegistrosObtenidos { get; set; }
}

public class UsuarioDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? RolNombre { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}
