using System.ComponentModel.DataAnnotations;

namespace LostPeople.Web.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(200)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Correo no válido")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Teléfono no válido")]
    [StringLength(20)]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
