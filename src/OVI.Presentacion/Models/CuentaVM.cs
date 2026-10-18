
using System.ComponentModel.DataAnnotations;

namespace OVI.Presentacion.Models;

public class RegistroVM
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = "";

    [Required, StringLength(100)]
    public string Apellido { get; set; } = "";

    [Required, EmailAddress, StringLength(150)]
    public string Correo { get; set; } = "";

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = "";

    [Required(ErrorMessage = "Confirma tu contraseña.")]
    [Compare(nameof(Contrasena), ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarContrasena { get; set; } = "";
}

public class LoginVM
{
    [Required, EmailAddress]
    public string Correo { get; set; } = "";

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = "";
}
