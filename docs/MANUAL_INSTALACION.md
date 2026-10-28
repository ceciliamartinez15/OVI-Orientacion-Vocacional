# Manual de instalación

## Requisitos
- Visual Studio 2022 con desarrollo web ASP.NET Core o SDK .NET 8.
- SQL Server Express/Developer y permisos para crear/modificar `OrientacionVocacionalDB`.
- `dotnet-ef` 8.x (`dotnet tool install --global dotnet-ef --version 8.0.11` si no está instalado).
- Opcional: Docker Desktop con Linux Containers.

## Clonar y abrir
`git clone <URL_DEL_REPOSITORIO>`; abrir `OrientacionVocacional.slnx`. Seleccionar `OVI.Presentacion` como proyecto de inicio.

## SQL Server local
En `src/OVI.Presentacion/appsettings.json`, revisar `ConnectionStrings:OVIConnection`. Por defecto es SQL Server Express local de la autora. No subir contraseñas a Git. Ejecutar desde la raíz:

```powershell
dotnet restore src/OVI.Presentacion/OVI.Presentacion.csproj
dotnet build src/OVI.Presentacion/OVI.Presentacion.csproj
dotnet ef database update --project src/OVI.Datos/OVI.Datos.csproj --startup-project src/OVI.Presentacion/OVI.Presentacion.csproj
dotnet run --project src/OVI.Presentacion/OVI.Presentacion.csproj
```

Si el proyecto local ya tiene base, realizar copia de seguridad antes de aplicar migraciones. Si aparece `MSB1009`, estás en una carpeta incorrecta; la terminal debe estar junto a `OrientacionVocacional.slnx`.

## Docker
Copiar `.env.example` a `.env` y cambiar `MSSQL_SA_PASSWORD`. Ejecutar `docker compose up --build -d`, verificar `docker compose ps`, `docker compose logs web` y abrir `http://localhost:8080`. La BD en Docker es distinta a la local. Para detener: `docker compose down`.

## Configuración IA
En local, definir `ChatbotIA__ApiKey`, `ChatbotIA__Endpoint`, `ChatbotIA__Modelo` como variables de entorno o secretos de usuario. No escribir claves en archivos versionados. Probar el **Asistente** (requiere haber hecho el test) con una pregunta; si responde que la IA no está configurada, falta la clave, el endpoint HTTPS o el modelo.
