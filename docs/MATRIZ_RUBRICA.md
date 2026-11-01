# Matriz de cumplimiento — Proyecto Fase II (2026-2)

Contrasta cada producto de la guía y cada criterio de la rúbrica con lo que hay en el repositorio. **Atención:** la guía exige el *Video de evidencia* (producto 8); su exclusión debe confirmarse por escrito con el docente. Estados: **Listo** · **Parcial** (existe, falta repetir pruebas o capturas) · **Pendiente** (depende del equipo).

## Productos esperados

| # | Producto de la guía | Dónde está | Estado |
|---|---|---|---|
| 1 | Aplicación web funcional: navegación, formularios con validación, usuarios, CRUD, EF Core, UX/UI de la Fase I | `src/OVI.Presentacion` (Views, Controllers) | Listo |
| 2 | Backend ASP.NET Core: capas, CRUD, BD, validaciones, excepciones, seguridad básica; opcional roles | 4 proyectos en `src/`; roles Admin y Estudiante; JWT y Web API no se usan (opcionales) | Listo |
| 3 | IA: entrada, procesamiento, resultados, beneficio | Asistente "Saviox" — [Manual técnico §6](MANUAL_TECNICO.md#6-integración-de-inteligencia-artificial) | Listo (P16–P19) |
| 4 | Docker y despliegue: Dockerfile, compose, variables de entorno; propuesta Azure con servicio, beneficios, costos y arquitectura | `docker/Dockerfile`, `docker-compose.yml`, `.env.example`, [PROPUESTA_AZURE](PROPUESTA_AZURE.md) | Listo; repetir P20 y P21 |
| 5 | Integración general: módulos, front-back, BD, IA | [Manual técnico §1](MANUAL_TECNICO.md#1-arquitectura-del-sistema) | Listo (P05–P08, P11, P19) |
| 6 | Pruebas: sistema, casos de uso, integración, IA, Docker, con evidencia | [PRUEBAS_Y_EVIDENCIAS](PRUEBAS_Y_EVIDENCIAS.md) (21 casos: 19 aprobados, 2 por repetir) | Parcial: repetir P20 y P21 (Docker) |
| 7 | Manual de instalación, de usuario y técnico | [Instalación](MANUAL_INSTALACION.md), [Usuario](MANUAL_USUARIO.md), [Técnico](MANUAL_TECNICO.md) | Listo, faltan capturas del manual de usuario |
| 8 | Video de evidencia (5–10 min, con todos los integrantes) | No existe en el repositorio | **Pendiente** (confirmar con el docente o grabarlo) |
| 9 | Repositorio: `/src` `/docs` `/database` `/docker`, código documentado, README, `.env.example`, commits, trabajo colaborativo | Estructura completa; README actualizado; ver Git | Listo; subir cambios recientes |
| — | Documento de avance técnico actualizado | [AVANCE_TECNICO](AVANCE_TECNICO.md) | Listo; completar aportes por integrante |

## Rúbrica de evaluación grupal (50 %)

| Criterio | Qué la satisface | Evidencia | Estado |
|---|---|---|---|
| Aplicación web funcional | Registro/login, test paso a paso, resultados, historial, panel por rol, CRUD, chats | P01–P15 | Listo |
| Backend y lógica de negocio | `ServicioVocacional` (validación de respuestas, puntajes, afinidad), autorización por roles, baja lógica | Manual técnico §4–5 | Listo |
| BD y Entity Framework Core | 10 tablas, relaciones, índices únicos, semillas, 4 migraciones | Manual técnico §4; P08, P15 | Listo |
| Operaciones CRUD | Usuarios, preguntas, carreras (con áreas RIASEC) e instituciones | P11–P14 | Listo |
| Integración de IA | Chat generativo con contexto del resultado, historial, manejo de errores | P16–P19 | Listo (P16–P19) |
| Integración general del sistema | Admin → catálogo → test → resultado → Asistente | P05–P08, P19 | Listo |
| Docker y despliegue | Dockerfile multietapa, compose con SQL Server y volumen, healthcheck, `/health`, propuesta Azure | P20, P21 | Listo |
| Diseño UX/UI | Interfaz VocalO (paleta marfil/café, mascota Saviox, test paso a paso, responsiva) coherente con el prototipo de la Fase I | Capturas del manual de usuario | Listo; **verificar** contra los diseños de la Fase I |
| Pruebas y validación | Plan de 21 casos en 3 bloques | `PRUEBAS_Y_EVIDENCIAS.md` | Parcial: repetir P20 y P21 |
| Documentación técnica | 3 manuales + Azure + avance técnico + matriz | `docs/` | Listo, faltan capturas |
| Repositorio Git | Estructura requerida, README, `.env.example`; 7 commits de 4 autores (a la fecha) | Git | Hacer commit de los cambios locales; cada integrante debe tener commits propios |
| Presentación final (pitch técnico) | Presentación presencial/en vivo con todos los integrantes | [GUION_PITCH](GUION_PITCH.md) | Guion listo; ensayar |

## Defensa individual (50 %) — informativo

Asistencia (10 %), test de contenidos del ciclo (30 %) y coevaluación grupal (10 %) no dependen del repositorio.

## Reglas de la guía a tener presentes

- Cada punto no presentado como se solicita resta 0.20 puntos a la nota de la actividad.
- No se puede disolver ni cambiar de grupo; quien no asista a la presentación recibe 0.
- Entrega tardía: −10 % por día de retraso hasta 3 días; después no se acepta o se califica con 0 (salvo justificación válida).

Esta matriz no promete una calificación: la nota depende de la evaluación docente y de que las pruebas y evidencias estén ejecutadas.
