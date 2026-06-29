using System.ComponentModel.DataAnnotations;

namespace LostPeople.Web.ViewModels;

public class ReportarViewModel
{
    [Required(ErrorMessage = "El primer nombre es obligatorio")]
    [StringLength(100)]
    public string PrimerNombre { get; set; } = string.Empty;

    [StringLength(100)]
    public string? SegundoNombre { get; set; }

    [Required(ErrorMessage = "El primer apellido es obligatorio")]
    [StringLength(100)]
    public string PrimerApellido { get; set; } = string.Empty;

    [StringLength(100)]
    public string? SegundoApellido { get; set; }

    [StringLength(100)]
    public string? Alias { get; set; }

    [DataType(DataType.Date)]
    public DateTime? FechaNacimiento { get; set; }

    [DataType(DataType.Date)]
    public DateTime? FechaDesaparicion { get; set; }

    [Range(0, 120, ErrorMessage = "La edad debe estar entre 0 y 120")]
    public int? EdadAproximada { get; set; }

    public string? Sexo { get; set; }

    [StringLength(2000)]
    public string? DescripcionFisica { get; set; }

    [Range(20, 250)]
    public decimal? EstaturaCm { get; set; }

    [StringLength(30)]
    public string? ColorPiel { get; set; }

    [StringLength(30)]
    public string? ColorOjos { get; set; }

    [StringLength(30)]
    public string? ColorCabello { get; set; }

    [StringLength(2000)]
    public string? SenasParticulares { get; set; }

    [StringLength(500)]
    public string? CondicionMedica { get; set; }

    [StringLength(500)]
    public string? MedicamentosRequeridos { get; set; }

    [StringLength(500)]
    public string? Vestimenta { get; set; }

    [StringLength(500)]
    public string? UltimaUbicacionTexto { get; set; }

    public decimal? UltimaUbicacionLat { get; set; }
    public decimal? UltimaUbicacionLng { get; set; }
    public int? UltimaUbicacionZonaId { get; set; }

    [StringLength(100)]
    public string? RelacionConDesaparecido { get; set; }

    [Phone(ErrorMessage = "El teléfono no es válido")]
    [StringLength(20)]
    public string? TelefonoContacto { get; set; }

    [EmailAddress(ErrorMessage = "El correo no es válido")]
    [StringLength(200)]
    public string? EmailContacto { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "Foto de la persona")]
    public IFormFile? FotoPersona { get; set; }

    public bool AceptoTerminos { get; set; }
    public bool AceptoConfidencialidad { get; set; }
}
