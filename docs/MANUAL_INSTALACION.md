# Manual de instalación

Guía para instalar y ejecutar OVI en un equipo nuevo, de forma local o con Docker.

## 1. Requisitos

| Herramienta | Versión | Para qué |
|---|---|---|
| Git | cualquiera reciente | Clonar el repositorio |
| .NET SDK | 8.0 | Compilar y ejecutar (opción local) |
| `dotnet-ef` | 8.0.x | Aplicar migraciones (opción local) |
| SQL Server Express / Developer | 2019 o superior | Base de datos (opción local) |
| Visual Studio 2022 (opcional) | con carga de trabajo *ASP.NET y desarrollo web* | IDE |
| Docker Desktop | Linux containers, ≥ 4 GB de RAM libres | Ejecución con contenedores |
| Cuenta en un proveedor de IA (opcional) | API compatible con *Chat Completions* (por defecto Groq) | Asistente con IA |

## 2. Clonar el repositorio

```powershell
git clone https://github.com/ceciliamartinez15/OVI-Orientacion-Vocacional.git
cd OVI-Orientacion-Vocacional
```

Abrir `OrientacionVocacional.slnx` en Visual Studio y marcar `OVI.Presentacion` como proyecto de inicio (solo opción local). Todos los comandos siguientes se ejecutan **desde esta carpeta** (junto a `OrientacionVocacional.slnx`); si aparece el error `MSB1009` estás en otra carpeta.

## 3. Instalación de dependencias

```powershell
dotnet tool install --global dotnet-ef --version 8.0.11   # solo si no está instalado
dotnet restore src/OVI.Presentacion/OVI.Presentacion.csproj
dotnet build   src/OVI.Presentacion/OVI.Presentacion.csproj
```

Paquetes NuGet principales (se restauran solos): `Microsoft.EntityFrameworkCore.SqlServer 8.0.11`, `Microsoft.EntityFrameworkCore.Design 8.0.11`, `Microsoft.EntityFrameworkCore.Tools 8.0.11`. Bootstrap 5.3.3 y las fuentes se cargan desde CDN, por lo que el navegador necesita Internet.

## 4. Variables de entorno y configuración

| Variable | Obligatoria | Descripción | Ejemplo / valor por defecto |
|---|---|---|---|
| `ConnectionStrings__OVIConnection` | No (local) | Cadena de conexión a SQL Server. Sustituye la de `appsettings.json` (por defecto `Server=localhost;...`) | `Server=.\SQLEXPRESS;Database=OrientacionVocacionalDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true` |
| `OVI_APPLY_MIGRATIONS` | No | `true` aplica las migraciones al iniciar (la usa Docker). En local se aplican a mano | `false` |
| `OVI_ADMIN_EMAIL` | No | Correo del primer administrador (solo se crea si ese correo no existe) | `admin@ovi.local` |
| `OVI_ADMIN_PASSWORD` | Con la anterior | Contraseña del administrador, **mínimo 12 caracteres** | — |
| `ChatbotIA__Endpoint` | Para IA | URL HTTPS del endpoint *chat completions* | `https://api.groq.com/openai/v1/chat/completions` |
| `ChatbotIA__Modelo` | Para IA | Identificador del modelo | `openai/gpt-oss-20b` |
| `ChatbotIA__ApiKey` | Para IA | Clave del proveedor. **Nunca subirla a Git** | — |
| `ASPNETCORE_ENVIRONMENT` | No | `Development` / `Production` | `Production` en Docker |

Variables del archivo `.env` (solo Docker; se crea copiando `.env.example`):

| Variable `.env` | Obligatoria | Se traduce a |
|---|---|---|
| `MSSQL_SA_PASSWORD` | **Sí** | Contraseña del usuario `sa` del contenedor SQL Server (12+ caracteres, mayúsculas, minúsculas, números y símbolos) |
| `OVI_ADMIN_EMAIL`, `OVI_ADMIN_PASSWORD` | No | Primer administrador |
| `IA_API_KEY` | Para IA | `ChatbotIA__ApiKey` |
| `IA_ENDPOINT`, `IA_MODEL` | No | `ChatbotIA__Endpoint`, `ChatbotIA__Modelo` (si quedan vacíos se usa Groq / `openai/gpt-oss-20b`) |

El archivo `.env` está en `.gitignore`; solo se versiona `.env.example`.

## 5. Ejecución local (sin Docker)

1. **Cadena de conexión.** Por defecto `src/OVI.Presentacion/appsettings.json` usa `Server=localhost` con autenticación de Windows y la base `OrientacionVocacionalDB`. Si tu instancia es otra (p. ej. `.\SQLEXPRESS`), usa secretos de usuario o la variable `ConnectionStrings__OVIConnection` (no escribas contraseñas en archivos versionados).
2. **Crear/actualizar la base** con las migraciones (no borrar una base existente; haz respaldo antes):

   ```powershell
   dotnet ef database update --project src/OVI.Datos/OVI.Datos.csproj --startup-project src/OVI.Presentacion/OVI.Presentacion.csproj
   ```

   Migraciones: `Inicial`, `AmpliarPerfilDominante`, `AgregarAreasCarrera`, `EliminarRolOrientador`. El archivo `database/01_crear_base.sql` es solo auxiliar; no lo ejecutes sobre una base ya migrada.
3. **Primer administrador** (una sola vez, en la misma terminal PowerShell donde ejecutarás la app):

   ```powershell
   $env:OVI_ADMIN_EMAIL="admin@ovi.local"
   $env:OVI_ADMIN_PASSWORD="UnaClaveMuySegura#2026"
   ```
4. **IA (opcional)**:

   ```powershell
   $env:ChatbotIA__Endpoint="https://api.groq.com/openai/v1/chat/completions"
   $env:ChatbotIA__Modelo="openai/gpt-oss-20b"
   $env:ChatbotIA__ApiKey="TU_CLAVE_PRIVADA"
   ```
5. **Ejecutar:**

   ```powershell
   dotnet run --project src/OVI.Presentacion/OVI.Presentacion.csproj
   ```

   Abrir la URL que muestra la consola (perfil `launchSettings.json`).

La cuenta semilla `demo@ovi.local` no tiene contraseña y **no puede iniciar sesión**. Los estudiantes se registran en `/Cuenta/Registro`.

## 6. Ejecución con Docker

```powershell
Copy-Item .env.example .env
# Editar .env con MSSQL_SA_PASSWORD (y opcionalmente IA_API_KEY, OVI_ADMIN_EMAIL, OVI_ADMIN_PASSWORD)
docker compose up --build -d
docker compose ps
docker compose logs -f web
```

- `db` (SQL Server 2022 Express) arranca primero; `web` espera su *healthcheck*, aplica las migraciones (hasta 12 reintentos cada 5 s) y crea el administrador si se configuró.
- Aplicación: `http://localhost:8080` · Salud: `http://localhost:8080/health`.
- Los datos persisten en el volumen `ovi_sql_data`. La base de Docker es independiente de la local.
- Detener: `docker compose down`. Reiniciar conservando datos: `docker compose restart`. **Evitar** `docker compose down -v` (borra la base).
- Reconstruir tras cambios de código: `docker compose up --build -d`.

## 7. Verificación rápida

1. `GET /health` responde `Healthy`.
2. Registrar un estudiante en `/Cuenta/Registro`, iniciar sesión, hacer el test y ver el resultado.
3. Iniciar sesión como administrador y abrir **Usuarios**, **Preguntas**, **Carreras**, **Instituciones**.
4. Con IA configurada: en **Asistente** (después de hacer el test) enviar una pregunta y comprobar que responde. Si dice que la IA no está configurada, falta clave, endpoint HTTPS o modelo.

## 8. Problemas frecuentes

| Síntoma | Causa y solución |
|---|---|
| `MSB1009` al compilar | Carpeta incorrecta; situarse junto a `OrientacionVocacional.slnx` |
| `Cannot open database` / error de login SQL | Revisar `OVIConnection`, que SQL Server esté en ejecución y que la migración se haya aplicado |
| `docker compose` exige `MSSQL_SA_PASSWORD` | Falta el archivo `.env` o la variable |
| El contenedor `web` reinicia | Contraseña de `sa` demasiado débil o `db` aún no saludable: ver `docker compose logs db web` |
| El Asistente responde "IA no configurada" | Falta `ChatbotIA__ApiKey` (o `IA_API_KEY` en `.env`); reiniciar tras cambiarla |
| El Asistente responde "problema para conectarme" | Clave o modelo inválidos, sin Internet o cuota agotada; ver el log del servidor |
| Pide iniciar sesión tras reiniciar el contenedor | Las cookies de sesión se invalidan sin *key ring* persistente (limitación conocida) |

## 9. Despliegue en Azure

La propuesta (servicio, beneficios, costos y arquitectura) está en [PROPUESTA_AZURE.md](PROPUESTA_AZURE.md). El repositorio incluye el workflow `.github/workflows/main_ovi-orientacion-vocacional.yml`, que compila con .NET 8 y publica en el App Service `ovi-orientacion-vocacional` al hacer *push* a `main`.
