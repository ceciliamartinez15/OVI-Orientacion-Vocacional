# OVI — versión ampliada (sobre el ZIP recibido)

## Antes de empezar
1. Haz copia de tu proyecto y de la base de datos. No borres la BD.
2. Abre la carpeta `proyecto de catedra` en Visual Studio 2022.
3. El proyecto usa SQL Server Express y mantiene el nombre `OVIConnection`.

## Migración de la columna PerfilDominante
Se incluye migración `20261014190000_AmpliarPerfilDominante` para pasar de nvarchar(10) a nvarchar(255).
**Si ya ejecutaste manualmente ALTER COLUMN a nvarchar(255)**, EF puede aplicar el cambio de nuevo; normalmente es compatible, pero realiza respaldo primero.

```powershell
dotnet restore src/OVI.Presentacion/OVI.Presentacion.csproj
dotnet build src/OVI.Presentacion/OVI.Presentacion.csproj
dotnet ef database update --project src/OVI.Datos/OVI.Datos.csproj --startup-project src/OVI.Presentacion/OVI.Presentacion.csproj
dotnet run --project src/OVI.Presentacion/OVI.Presentacion.csproj
```

## Crear el primer administrador (solo una vez)
La cuenta demo antigua no tiene contraseña y no permite iniciar sesión.
En la MISMA terminal PowerShell, antes de `dotnet run`, ejecuta:
```powershell
$env:OVI_ADMIN_EMAIL="admin@ovi.local"
$env:OVI_ADMIN_PASSWORD="CAMBIA_ESTA_CLAVE_POR_UNA_MUY_SEGURA"
dotnet run --project src/OVI.Presentacion/OVI.Presentacion.csproj
```
El sistema solo crea el administrador si ese correo no existe. No convierte cuentas existentes en administradores.
Después de crear el admin puedes borrar las variables de entorno de la sesión con `Remove-Item Env:OVI_ADMIN_PASSWORD`.
Los estudiantes se registran en `/Cuenta/Registro`.

## IA real (opcional hasta configurar proveedor)
El chatbot (tomado de DSP1) tiene dos chats: **Preguntas frecuentes** (`/Chatbot/Faq`, reglas sin IA, abierto a todos) y **Asistente** (`/Chatbot/Contextual`, IA generativa sobre tu último resultado, requiere sesión).
Para activar IA en PowerShell, configura variables de entorno antes de ejecutar:
```powershell
$env:ChatbotIA__Endpoint="https://api.groq.com/openai/v1/chat/completions"
$env:ChatbotIA__Modelo="openai/gpt-oss-20b"
$env:ChatbotIA__ApiKey="TU_CLAVE_PRIVADA"
```
La clave **nunca** debe subirse a Git. Verifica disponibilidad y costos del proveedor.
Si no se configura, el Asistente indica que la IA no está configurada y las Preguntas frecuentes siguen funcionando.

## Qué incluye
- Registro/login/logout con contraseñas hasheadas y cookies.
- Permisos: Admin, Estudiante y Orientador (no se crean orientadores automáticamente).
- Test privado y resultados protegidos por usuario.
- Historial personal, CRUD de carreras, preguntas e instituciones, gestión de estado de usuarios.
- Corrección del campo PerfilDominante y validación de respuestas completas.
- Chatbot de DSP1: FAQ por palabras clave + chat con IA contextual (historial de 6 turnos en sesión).

## Pendiente para cumplir toda la rúbrica
- Ejecutar pruebas completas en Windows con SQL Server (no se compiló en el entorno de generación).
- Pruebas automatizadas, UX/UI comparado con Fase I, documentación completa, Docker, evidencias, Git y pitch.
- La IA requiere credenciales válidas y conectividad; sin eso no cuenta como IA integrada.
- Las recomendaciones RIASEC son reglas académicas orientativas, no un test psicométrico validado.
