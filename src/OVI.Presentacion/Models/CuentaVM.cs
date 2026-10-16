using System.ComponentModel.DataAnnotations;
namespace OVI.Presentacion.Models;
public class RegistroVM
{
 [Required,StringLength(100)] public string Nombre {get;set;}="";
 [Required,StringLength(100)] public string Apellido {get;set;}="";
 [Required,EmailAddress,StringLength(150)] public string Correo {get;set;}="";
 [Required,MinLength(8),DataType(DataType.Password)] public string Contrasena {get;set;}="";
 [Required,Compare(nameof(Contrasena)),DataType(DataType.Password)] public string ConfirmarContrasena {get;set;}="";
}
public class LoginVM
{
 [Required,EmailAddress] public string Correo {get;set;}="";
 [Required,DataType(DataType.Password)] public string Contrasena {get;set;}="";
}
