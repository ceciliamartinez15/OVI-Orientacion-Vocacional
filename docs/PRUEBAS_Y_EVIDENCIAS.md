# Plan de pruebas y evidencias

**No marcar como aprobado hasta ejecutarlo y capturar evidencia real.** Guardar capturas en `docs/evidencias/` y anotar fecha, responsable, resultado esperado y observado.

| ID | Caso | Resultado esperado | Evidencia |
|---|---|---|---|
| P01 | Registro de estudiante nuevo | Cuenta creada con rol Estudiante | Captura de registro e inicio |
| P02 | Login con contraseña incorrecta | Rechazo sin iniciar sesión | Captura de error |
| P03 | Login correcto y cierre de sesión | Acceso y cierre de sesión | Capturas |
| P04 | Estudiante intenta entrar a `/Usuarios` | Acceso denegado | Captura HTTP 403 o página de acceso denegado |
| P05 | Estudiante ve inicio | Solo métricas personales | Captura |
| P06 | Admin ve inicio y CRUD | Métricas globales y controles CRUD | Captura |
| P07 | Crear, editar y eliminar carrera | Cambios reflejados en catálogo | Capturas antes/después |
| P08 | Test incompleto | Validación impide guardar | Captura |
| P09 | Test completo | Perfil, puntajes y recomendaciones guardados | Captura y SELECT SQL |
| P10 | Usuario A intenta ver resultado de B | Acceso denegado | Captura |
| P11 | Chat FAQ sin clave | Respuesta marcada como FAQ, no IA | Captura |
| P12 | Chat con proveedor IA configurado | Respuesta marcada como IA, mostrar entrada/proceso/salida | Captura + registro de configuración sin claves |
| P13 | Docker web y SQL | `docker compose ps` ambos en marcha; web accesible en 8080 | Capturas consola/navegador |
| P14 | Persistencia Docker | Reiniciar contenedores, datos permanecen | Captura antes/después |
| P15 | Impresión resultado | PDF guardado desde navegador | Captura PDF |

Consulta SQL de apoyo:
```sql
USE OrientacionVocacionalDB;
SELECT COUNT(*) AS Usuarios FROM dbo.Usuarios;
SELECT COUNT(*) AS Evaluaciones FROM dbo.Evaluaciones;
SELECT TOP (5) PerfilDominante, FechaResultado FROM dbo.Resultados ORDER BY FechaResultado DESC;
```
