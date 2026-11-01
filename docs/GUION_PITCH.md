# Guion de la presentación final (pitch técnico)

Duración sugerida: 10–12 minutos. **Todos los integrantes deben hablar**; la rúbrica evalúa participación y dominio del proyecto de cada uno.

| # | Bloque | Contenido | Tiempo | Expone |
|---|---|---|---|---|
| 1 | Problema | Estudiantes sin orientación clara para elegir carrera; necesidad de una herramienta accesible | 1 min | _[Completar]_ |
| 2 | Solución | VocalO: test RIASEC de 12 afirmaciones, 3 carreras recomendadas, historial, administración de catálogos | 1 min | _[Completar]_ |
| 3 | Arquitectura | 4 capas (Presentacion, Logica, Datos, Modelos), EF Core, SQL Server; diagrama del Manual técnico §1 | 2 min | _[Completar]_ |
| 4 | Demo estudiante | Registro, test, resultado, historial, PDF | 2 min | _[Completar]_ |
| 5 | Demo administrador | CRUD de usuarios, preguntas, carreras (áreas RIASEC), instituciones; estadísticas | 2 min | _[Completar]_ |
| 6 | IA | Entrada (perfil + 3 carreras + pregunta), procesamiento (prompt y API Chat Completions), resultado, beneficio, casos controlados | 2 min | _[Completar]_ |
| 7 | Docker y Azure | `docker compose up --build -d`, healthcheck, volumen; propuesta App Service + Azure SQL y costos (≈ 23–26 USD/mes) | 1.5 min | _[Completar]_ |
| 8 | Retos y lecciones | Secretos, sincronización de documentación, límites de la heurística RIASEC | 1 min | _[Completar]_ |
| 9 | Cierre y preguntas | Resultados de pruebas (19/21 aprobadas, Docker por repetir), trabajo futuro | 0.5 min | _[Completar]_ |

## Antes de presentar

1. `docker compose up --build -d` y comprobar `http://localhost:8080/health`.
2. Tener una cuenta de administrador y una de estudiante con test realizado.
3. Tener `IA_API_KEY` válida en `.env` (no mostrar el archivo en pantalla).
4. Cada integrante debe poder explicar cualquier módulo: preguntas típicas son cómo se calcula la afinidad, cómo se protege la sesión, qué hace `OVI_APPLY_MIGRATIONS` y por qué la IA no genera las recomendaciones.
