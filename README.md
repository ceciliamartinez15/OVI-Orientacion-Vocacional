# OVI · Orientación Vocacional

Aplicación académica ASP.NET Core MVC (.NET 8), SQL Server y Entity Framework Core. Incluye registro e inicio de sesión con hash de contraseña, perfiles Estudiante/Admin/Orientador, test RIASEC, historial, recomendaciones, catálogo y CRUD administrativos, asistente FAQ y **conector opcional a IA generativa**.

> El modo FAQ **no es IA generativa**. Para demostrar IA real en la rúbrica hay que configurar y probar un endpoint compatible con Chat Completions, con su API key y modelo.

## Ejecutar en Windows con tu SQL Server Express

1. Abrir `OrientacionVocacional.slnx` en Visual Studio 2022. Seleccionar `OVI.Presentacion` como proyecto de inicio.
2. Revisar `src/OVI.Presentacion/appsettings.json`: cadena `OVIConnection`. Por defecto apunta a `michelle\sqlexpress` y base `OrientacionVocacionalDB`. Cambiar el servidor si se usa otro equipo.
3. Desde esta carpeta:

```powershell
dotnet restore src/OVI.Presentacion/OVI.Presentacion.csproj
dotnet ef database update --project src/OVI.Datos/OVI.Datos.csproj --startup-project src/OVI.Presentacion/OVI.Presentacion.csproj
dotnet run --project src/OVI.Presentacion/OVI.Presentacion.csproj
```

La migración es incremental: no borrar la base existente. Realizar respaldo antes de migrar.

## Ejecutar con Docker (base independiente)

Requiere Docker Desktop en modo Linux Containers, al menos 4 GB de memoria disponibles y conexión a Internet para obtener las imágenes.

```powershell
Copy-Item .env.example .env
# Editar .env y reemplazar MSSQL_SA_PASSWORD por una contraseña fuerte
# Opcional: completar IA_API_KEY, IA_ENDPOINT e IA_MODEL
# Opcional: completar OVI_ADMIN_EMAIL y OVI_ADMIN_PASSWORD para una cuenta admin nueva
docker compose up --build -d
docker compose logs -f web
```

Abrir `http://localhost:8080`. El contenedor crea/aplica migraciones en una **base SQL Server propia**, guardada en el volumen `ovi_sql_data`. No utiliza ni modifica la instancia local `michelle\sqlexpress`.

Para detener: `docker compose down`. **No usar** `docker compose down -v` salvo que quieras eliminar la base del contenedor.

## Administrador

El rol `Admin` puede gestionar carreras, preguntas, instituciones y usuarios. El registro público solo crea estudiantes. Para crear un administrador NUEVO mediante configuración, completar `OVI_ADMIN_EMAIL` y `OVI_ADMIN_PASSWORD` (mínimo 12 caracteres) antes del primer inicio. Si la cuenta ya existe, **no se eleva su rol automáticamente**. En tu SQL Server local, puedes continuar usando la cuenta admin que ya configuraste.

## IA generativa

Usa un servicio HTTPS compatible con el formato `POST /chat/completions`, con Bearer token y JSON `choices[0].message.content`. Configurar variables de entorno (no subir claves al repositorio):

- `IA__Endpoint`: URL completa HTTPS del endpoint de chat completions.
- `IA__Model`: identificador del modelo permitido por el proveedor.
- `IA__ApiKey`: clave del proveedor.

En Docker se rellenan desde `IA_ENDPOINT`, `IA_MODEL` e `IA_API_KEY` del `.env`. Si faltan o falla la conexión, se muestra explícitamente **FAQ sin IA**. El uso de la API puede generar costos. No se incluye una clave ni un servicio contratado.

## Documentos

- `docs/MANUAL_INSTALACION.md`
- `docs/MANUAL_USUARIO.md`
- `docs/MANUAL_TECNICO.md`
- `docs/PRUEBAS_Y_EVIDENCIAS.md`
- `docs/PROPUESTA_AZURE.md`
- `docs/GUION_VIDEO.md`
- `docs/MATRIZ_RUBRICA.md`

## Limitaciones conocidas

- La evaluación RIASEC es un prototipo de exploración, no un instrumento psicométrico validado.
- La afinidad de carreras es heurística; no garantiza resultados profesionales.
- La exportación a PDF utiliza la opción **Imprimir / Guardar PDF** del navegador.
- El proyecto requiere pruebas reales en Visual Studio, Docker y con proveedor de IA antes de su entrega.
- Para producción: añadir recuperación de contraseña, políticas de bloqueo, confirmación de correo, auditoría, protección de datos y un proveedor persistente de claves de Data Protection entre reinicios de contenedores.
