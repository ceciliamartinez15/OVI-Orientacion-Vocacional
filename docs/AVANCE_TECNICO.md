# Documento de avance técnico — OVI · Orientación Vocacional (VocalO)

Proyecto Fase II · Escuela de Computación, Universidad Don Bosco · Ciclo 2026-2 · Actualizado al 31/10/2026.

## 1. Equipo

| Integrante | Aporte principal (completar) | Commits en Git |
|---|---|---|
| Vanessa Guadalupe Montes Rosa | _[Completar]_ | _[Completar]_ |
| Josué Israel García Ayala | _[Completar]_ | _[Completar]_ |
| Valeria Alexandra Carcamo López | _[Completar]_ | _[Completar]_ |
| Samuel Isaac López Núñez | _[Completar]_ | 2 |
| Roberto Bezaleel Portillo Montano | Plan de pruebas y evidencias | 1 |
| Cecilia Michelle Martínez Méndez | Entrega inicial y workflow de Azure | 4 (cuentas `miche` y `ceciliamartinez15`) |

Coordinador (quien realiza las entregas): _[Vanessa Guadalupe Montes Rosa]_.

## 2. Resumen del avance

| Área | Estado | Detalle |
|---|---|---|
| Aplicación web (ASP.NET Core MVC, .NET 8) | Completa | Registro/login, test RIASEC paso a paso, resultado, historial, panel por rol, estadísticas |
| Backend en capas | Completo | `OVI.Presentacion`, `OVI.Logica`, `OVI.Datos`, `OVI.Modelos` |
| Base de datos y EF Core | Completa | 10 tablas, 4 migraciones, datos semilla, índices únicos |
| CRUD | Completo | Usuarios, preguntas, carreras (con áreas RIASEC) e instituciones, con baja lógica |
| Seguridad básica | Completa | Cookies, hash de contraseñas, antiforgery, roles Estudiante y Admin, revalidación de sesión |
| Inteligencia Artificial | Completa | Asistente "Saviox" (API Chat Completions) con el perfil y las 3 carreras del último resultado |
| Docker | Completo | `Dockerfile` multietapa, `docker-compose.yml` con SQL Server, volumen y healthcheck, `/health` |
| Propuesta Azure | Completa | App Service para contenedores + Azure SQL; ver `PROPUESTA_AZURE.md` |
| Manuales | Completos en texto | Faltan las capturas del manual de usuario (`docs/capturas/`) |
| Pruebas y evidencias | 19 de 21 casos aprobados | P20 y P21 (Docker) por repetir con la versión actual |
| Video de evidencia | Pendiente de confirmar | La guía lo exige; confirmar con el docente |

## 3. Historial de hitos (según Git)

| Fecha | Hito |
|---|---|
| 16/10/2026 | Entrega Fase II inicial con Docker; workflow de GitHub Actions para Azure App Service |
| 18/10/2026 | Corrección de commit |
| 24/10/2026 | Actualización de documentación: evidencias, matriz de rúbrica y pruebas |
| 28/10/2026 | Nuevo diseño (VocalO) e integración de IA generativa |
| 31/10/2026 | Panel de carreras con áreas RIASEC, gestión de usuarios, retiro del rol Orientador, revisión de manuales y evidencias |

## 4. Decisiones técnicas

- Arquitectura monolítica modular en cuatro capas, para separar presentación, lógica, datos y modelo.
- SQL Server con EF Core *code first*; las migraciones son la fuente de verdad del esquema.
- La IA solo **explica** resultados: las recomendaciones de carrera las calcula el sistema con reglas determinísticas.
- La clave de la IA se entrega por variable de entorno (`ChatbotIA__ApiKey` / `IA_API_KEY`), nunca versionada.
- El chat de Preguntas frecuentes es por reglas y no se presenta como IA.

## 5. Retos y lecciones aprendidas

- Mantener la documentación sincronizada con el código: varios cambios de diseño obligaron a repetir capturas.
- Gestión de secretos: la clave del proveedor de IA debe vivir solo en `.env` o en el gestor de secretos del servidor.
- Las sesiones y cookies no persisten entre reinicios del contenedor sin un *key ring* compartido (limitación conocida).

## 6. Pendientes

- Tomar las capturas `u01`–`u15` del manual de usuario.
- Repetir las pruebas P20 y P21 de Docker con la versión actual.
- Completar aportes por integrante y commits propios de cada uno.
- Confirmar con el docente el video de evidencia.
- Ensayar la presentación final (`GUION_PITCH.md`).
