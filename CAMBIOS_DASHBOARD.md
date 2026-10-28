# Actualización: panel por roles

Esta versión modifica solamente `HomeController.cs` y `Views/Home/Index.cshtml`.

- Administrador y orientador: métricas generales.
- Estudiante: solo número de sus evaluaciones y su último perfil.
- Visitante: no ve estadísticas.
- No cambia la base de datos ni requiere migraciones.

## Instalar
1. Haz copia de seguridad de la carpeta actual.
2. Abre esta solución nueva desde la carpeta extraída.
3. Verifica la cadena `OVIConnection` en `src/OVI.Presentacion/appsettings.json` para tu instancia SQL Server.
4. Ejecuta `dotnet build src/OVI.Presentacion/OVI.Presentacion.csproj`.
5. Ejecuta `dotnet run --project src/OVI.Presentacion/OVI.Presentacion.csproj`.

No ejecutar nuevas migraciones para este cambio. La seguridad de rutas sigue dependiendo de los atributos de autorización de los controladores.
