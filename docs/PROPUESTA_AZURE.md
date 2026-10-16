# Propuesta de despliegue en Microsoft Azure

**La rúbrica exige documentar una propuesta; no obliga a desplegar realmente en Azure.**

## Arquitectura propuesta
Usuario → HTTPS → Azure App Service (contenedor web ASP.NET Core) → Azure SQL Database. Secretos en Azure Key Vault; registros en Application Insights. El servicio IA puede ser Azure OpenAI si existe acceso aprobado, o un proveedor HTTPS compatible.

## Beneficios
Escalado administrado, TLS, monitoreo, copias de seguridad, integración con GitHub Actions y separación de credenciales. Se recomienda identidad administrada para acceder a servicios compatibles.

## Costos
Dependen de región, plan, uso y precios vigentes. Para el informe, consultar la **calculadora oficial de Azure** y adjuntar una cotización fechada para App Service, Azure SQL Database, almacenamiento, monitoreo y uso del modelo IA. No se proporcionan cifras fijas porque pueden cambiar y no se han verificado precios actuales.

## Pasos de implementación propuestos
1. Crear grupo de recursos, registro de contenedores y App Service.
2. Publicar la imagen de `docker/Dockerfile` en Azure Container Registry.
3. Provisionar Azure SQL Database y aplicar migraciones de manera controlada.
4. Configurar cadena de conexión, secretos de IA y HTTPS.
5. Configurar registros, alertas, backup y validar autenticación, CRUD, test y chat.

No subir `.env`, credenciales ni claves API a Git.
