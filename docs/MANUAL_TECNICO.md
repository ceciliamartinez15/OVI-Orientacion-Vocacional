# Manual técnico

## Arquitectura
- `OVI.Presentacion`: ASP.NET Core MVC, controladores, vistas Razor, autenticación por cookies y servicio HTTP de IA.
- `OVI.Logica`: cálculo de puntuaciones RIASEC y afinidad de carreras.
- `OVI.Datos`: DbContext de Entity Framework Core, relaciones, semillas y migraciones.
- `OVI.Modelos`: entidades y restricciones.
- `database`: SQL de apoyo; migraciones EF Core son la fuente de verdad para la creación de tablas.
- `docker`: Dockerfile multietapa.

## Datos
SQL Server con tablas Roles, Instituciones, Usuarios, Evaluaciones, PreguntasRIASEC, Respuestas, Resultados, Carreras, Recomendaciones, FAQChatbot. Migraciones inicial y ampliación de PerfilDominante a 255 caracteres.

## Seguridad
Contraseñas hasheadas con `IPasswordHasher<Usuario>`, cookies HTTP-only, antiforgery en formularios POST, `[Authorize(Roles=...)]` en controladores. El registro público asigna exclusivamente Estudiante. Las páginas de resultados verifican el dueño. El admin inicial se crea únicamente cuando se proporciona un correo nuevo y una contraseña de al menos 12 caracteres.

## IA
`ChatbotController` expone dos chats por AJAX (JSON): **FAQ** (`ChatbotFAQService`, coincidencia de palabras clave sin tildes ni stopwords sobre las preguntas de la tabla `FAQChatbot` y un banco base; sin IA; acceso anónimo) y **Asistente** (`ChatbotIAService`, solo autenticados). El Asistente arma un prompt con el perfil dominante y las recomendaciones del último `Resultado` del usuario, añade hasta 6 turnos de historial guardados en Session (por usuario y resultado) y llama por backend al endpoint HTTPS configurado en la sección `ChatbotIA` (`Endpoint`, `Modelo`, `ApiKey`, autenticación Bearer, formato chat completions, timeout 18 s). Los errores de proveedor se registran en log y al usuario se le muestra un mensaje genérico. Los POST llevan token antiforgery en la cabecera `RequestVerificationToken`. Si falta la configuración, el Asistente lo indica sin llamar al proveedor.

## Docker
`docker-compose.yml` contiene web y SQL Server 2022, volumen persistente y healthcheck TCP. `OVI_APPLY_MIGRATIONS=true` habilita migración automática **solo en el contenedor**. En local no se aplican automáticamente. `docker/Dockerfile` compila y publica .NET 8 en dos etapas. Puerto web 8080.

## Operación y limitaciones
La cookie no garantiza persistencia entre reinicios de contenedores sin configurar un key ring compartido. Para uso real añadir gestión robusta de cuentas, logging/auditoría, HTTPS mediante proxy inverso, gestión de secretos, copias de seguridad y revisión de seguridad.
