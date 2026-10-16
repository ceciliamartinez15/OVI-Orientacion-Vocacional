# Manual técnico

## Arquitectura
- `OVI.Presentacion`: ASP.NET Core MVC, controladores, vistas Razor, autenticación por cookies y servicio HTTP de IA.
- `OVI.Logica`: cálculo de puntuaciones RIASEC, afinidad de carreras y FAQ.
- `OVI.Datos`: DbContext de Entity Framework Core, relaciones, semillas y migraciones.
- `OVI.Modelos`: entidades y restricciones.
- `database`: SQL de apoyo; migraciones EF Core son la fuente de verdad para la creación de tablas.
- `docker`: Dockerfile multietapa.

## Datos
SQL Server con tablas Roles, Instituciones, Usuarios, Evaluaciones, PreguntasRIASEC, Respuestas, Resultados, Carreras, Recomendaciones, FAQChatbot. Migraciones inicial y ampliación de PerfilDominante a 255 caracteres.

## Seguridad
Contraseñas hasheadas con `IPasswordHasher<Usuario>`, cookies HTTP-only, antiforgery en formularios POST, `[Authorize(Roles=...)]` en controladores. El registro público asigna exclusivamente Estudiante. Las páginas de resultados verifican el dueño. El admin inicial se crea únicamente cuando se proporciona un correo nuevo y una contraseña de al menos 12 caracteres.

## IA
`ServicioIA` envía mensaje y contexto del último perfil RIASEC al endpoint HTTPS configurado y lee respuesta de chat completions. Fallback explícito a FAQ si no se configura o hay error. El endpoint no debe exponer credenciales al navegador; el servicio se invoca desde backend. La respuesta debe validarse con un proveedor real durante las pruebas.

## Docker
`docker-compose.yml` contiene web y SQL Server 2022, volumen persistente y healthcheck TCP. `OVI_APPLY_MIGRATIONS=true` habilita migración automática **solo en el contenedor**. En local no se aplican automáticamente. `docker/Dockerfile` compila y publica .NET 8 en dos etapas. Puerto web 8080.

## Operación y limitaciones
La cookie no garantiza persistencia entre reinicios de contenedores sin configurar un key ring compartido. Para uso real añadir gestión robusta de cuentas, logging/auditoría, HTTPS mediante proxy inverso, gestión de secretos, copias de seguridad y revisión de seguridad.
