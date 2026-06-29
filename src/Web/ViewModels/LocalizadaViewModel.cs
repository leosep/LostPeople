using System.ComponentModel.DataAnnotations;

namespace LostPeople.Web.ViewModels;

public class LocalizadaViewModel
{
    [StringLength(20)]
    public string? CodigoSeguimiento { get; set; }

    [StringLength(200)]
    public string? NombrePersona { get; set; }

    [StringLength(200)]
    public string? TuNombre { get; set; }

    [Phone]
    [StringLength(20)]
    public string? TuTelefono { get; set; }

    public bool EsFamiliar { get; set; }

    [StringLength(500)]
    public string? LugarLocalizacion { get; set; }

    public bool EstaSalvo { get; set; } = true;
}
