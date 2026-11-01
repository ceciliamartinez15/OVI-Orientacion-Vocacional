# OVI · Orientación Vocacional (marca visible: VocalO)

Aplicación web académica (Proyecto Fase II) para orientación vocacional de estudiantes: test **RIASEC**, recomendación de carreras, historial, panel de estadísticas, administración completa del catálogo y dos chats — **Preguntas frecuentes** (reglas, sin IA) y **Asistente "Saviox"** (IA generativa sobre el último resultado del estudiante).

**Stack:** ASP.NET Core MVC (.NET 8, C#) · Entity Framework Core 8 · SQL Server · Docker / Docker Compose · IA vía API *Chat Completions* (Groq por defecto) · propuesta de despliegue en Microsoft Azure.

> El chat de **Preguntas frecuentes** no es IA generativa. Solo el **Asistente** usa IA, y únicamente funciona si se configura `ChatbotIA__ApiKey` (ver [Manual de instalación](docs/MANUAL_INSTALACION.md)).


| Equipo |
| :--- |
| Vanessa Guadalupe Montes Rosa |
| Josué Israel García Ayala |
| Valeria Alexandra Carcamo López |
| Samuel Isaac López Núñez |
| Roberto Bezaleel Portillo Montano |
| Cecilia Michelle Martínez Méndez |

## Estructura del repositorio

```
/src        Código fuente (4 proyectos: Presentacion, Logica, Datos, Modelos)
/docs       Manuales, pruebas, evidencias, propuesta Azure, matriz de rúbrica, avance técnico
/database   Script SQL auxiliar (las migraciones de EF Core son la fuente de verdad)
/docker     Dockerfile (docker-compose.yml y .env.example están en la raíz)
```

## Inicio rápido con Docker (recomendado)

Requiere Docker Desktop (Linux containers), ≥ 4 GB de RAM libre e Internet.

```powershell
git clone https://github.com/ceciliamartinez15/OVI-Orientacion-Vocacional.git
cd OVI-Orientacion-Vocacional
Copy-Item .env.example .env
# Editar .env: MSSQL_SA_PASSWORD (obligatorio), IA_API_KEY (para el Asistente),
# OVI_ADMIN_EMAIL / OVI_ADMIN_PASSWORD (para crear el primer administrador)
docker compose up --build -d
docker compose logs -f web
```

Abrir `http://localhost:8080`. Estado del servicio: `http://localhost:8080/health`.
El contenedor aplica las migraciones y usa su propia base SQL Server (volumen `ovi_sql_data`).
Detener: `docker compose down` (**no** usar `-v` salvo que quieras borrar la base).

## Ejecución local (Visual Studio / SQL Server Express)

Ver [docs/MANUAL_INSTALACION.md](docs/MANUAL_INSTALACION.md). Resumen:

```powershell
dotnet restore src/OVI.Presentacion/OVI.Presentacion.csproj
dotnet ef database update --project src/OVI.Datos/OVI.Datos.csproj --startup-project src/OVI.Presentacion/OVI.Presentacion.csproj
dotnet run --project src/OVI.Presentacion/OVI.Presentacion.csproj
```

La cadena `OVIConnection` de `appsettings.json` apunta por defecto a `localhost` con autenticación de Windows; cámbiala (o usa `ConnectionStrings__OVIConnection`, p. ej. `.\SQLEXPRESS`) si tu instancia es distinta.

## Roles y funcionalidades

| Rol | Puede |
|---|---|
| Visitante | Ver la página de inicio, registrarse, iniciar sesión y usar las Preguntas frecuentes |
| Estudiante | Realizar el test RIASEC, ver resultados e historial propios, usar el Asistente con IA |
| Admin | CRUD (con baja lógica) de usuarios, preguntas, carreras e instituciones; panel de estadísticas |

El registro público solo crea estudiantes. El primer administrador se crea con `OVI_ADMIN_EMAIL` y `OVI_ADMIN_PASSWORD` (≥ 12 caracteres); nunca se eleva el rol de una cuenta existente.

## IA

El Asistente envía a un endpoint HTTPS compatible con `POST /chat/completions` el perfil dominante y las 3 carreras recomendadas del último resultado del usuario, más hasta 6 turnos de historial. Variables: `ChatbotIA__Endpoint`, `ChatbotIA__Modelo`, `ChatbotIA__ApiKey` (en Docker: `IA_ENDPOINT`, `IA_MODEL`, `IA_API_KEY`). **No subir claves al repositorio.** Detalle de entradas, procesamiento y resultados en el [Manual técnico](docs/MANUAL_TECNICO.md#6-integración-de-inteligencia-artificial).

## Documentación

| Documento | Contenido |
|---|---|
| [MANUAL_INSTALACION](docs/MANUAL_INSTALACION.md) | Clonación, dependencias, variables de entorno, ejecución local y con Docker |
| [MANUAL_USUARIO](docs/MANUAL_USUARIO.md) | Acceso, funcionalidades por rol y uso de la IA |
| [MANUAL_TECNICO](docs/MANUAL_TECNICO.md) | Arquitectura, tecnologías, estructura, configuración, IA y Docker |
| [PRUEBAS_Y_EVIDENCIAS](docs/PRUEBAS_Y_EVIDENCIAS.md) | Plan y resultados de pruebas (capturas en `docs/evidencias/`) |
| [PROPUESTA_AZURE](docs/PROPUESTA_AZURE.md) | Servicio, beneficios, costos aproximados y arquitectura de despliegue |
| [AVANCE_TECNICO](docs/AVANCE_TECNICO.md) | Documento de avance técnico actualizado |
| [GUION_PITCH](docs/GUION_PITCH.md) | Guion de la presentación final (pitch técnico) |
| [MATRIZ_RUBRICA](docs/MATRIZ_RUBRICA.md) | Cumplimiento punto por punto de la guía y la rúbrica |


## Limitaciones conocidas

- El test RIASEC (12 afirmaciones) es una herramienta orientativa, no un instrumento psicométrico validado; la afinidad de carreras es heurística.
- La exportación a PDF usa **Imprimir / Guardar PDF** del navegador.
- El Asistente requiere una API key válida, conectividad y puede generar costos según el proveedor.

