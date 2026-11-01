using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OVI.Presentacion.Models;

// Formulario de administración de usuarios (crear y editar)
public class UsuarioFormVM
{
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio."), StringLength(100)]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El apellido es obligatorio."), StringLength(100)]
    public string Apellido { get; set; } = "";

    [Required(ErrorMessage = "El correo es obligatorio."), EmailAddress(ErrorMessage = "Correo no válido."), StringLength(150)]
    public string Correo { get; set; } = "";

    [Display(Name = "Rol")]
    public int IdRol { get; set; } = 1;

    [Display(Name = "Institución")]
    public int IdInstitucion { get; set; } = 1;

    [DataType(DataType.Password), Display(Name = "Contraseña")]
    public string? Contrasena { get; set; }

    public bool Estado { get; set; } = true;

    public List<SelectListItem> Roles { get; set; } = new();
    public List<SelectListItem> Instituciones { get; set; } = new();
}

// Formulario de administración de carreras (crear y editar)
public class CarreraFormVM
{
    public static readonly string[] AreasDisponibles = ["Realista", "Investigador", "Artistico", "Social", "Emprendedor", "Convencional"];
    public static readonly string[] Tipos = ["Universitaria", "Técnica", "Tecnológica"];

    public int IdCarrera { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio."), StringLength(150)]
    public string Nombre { get; set; } = "";

    [StringLength(500)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required, StringLength(50)]
    public string Tipo { get; set; } = "Universitaria";

    [StringLength(150)]
    [Display(Name = "Institución")]
    public string? Institucion { get; set; }

    [Display(Name = "Áreas RIASEC")]
    public List<string> Areas { get; set; } = new();

    public bool Estado { get; set; } = true;
}
