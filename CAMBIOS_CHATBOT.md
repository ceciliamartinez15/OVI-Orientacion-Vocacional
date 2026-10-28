# Chatbot de DSP1 integrado en OVI

Se reemplazó el chat anterior (`ChatController`, `ServicioIA`, `ServicioChatbot`, `Views/Chat`) por el chatbot de DSP1.

## Nuevo
- `Controllers/ChatbotController.cs`: `Faq`, `PreguntarFaq` (JSON), `Contextual`, `PreguntarIA` (JSON).
- `Servicios/ChatbotFAQService.cs`: FAQ por palabras clave (sin tildes, con stopwords). Usa las preguntas de la tabla `FAQChatbot` y un banco base adaptado a OVI.
- `Servicios/ChatbotIAService.cs` y `ChatbotIAOptions.cs`: IA con Chat Completions (Bearer), historial de 6 turnos, timeout de 18 s.
- `Models/ChatbotModelos.cs`: contexto vocacional, turnos de conversación y claves de sesión.
- `Views/Chatbot/Faq.cshtml` y `Contextual.cshtml`, `wwwroot/js/chatbot.js`, `wwwroot/css/chatbot.css`.

## Diferencias con DSP1 (por la arquitectura de OVI)
- El contexto del Asistente sale de la base de datos (último `Resultado` + `Recomendaciones` del usuario), no de Session.
- El historial se guarda en Session por usuario y resultado.
- Los POST llevan token antiforgery; el mensaje se limita a 500 caracteres.
- El Asistente exige sesión; Preguntas frecuentes es público.
- Si falta la configuración de IA, el Asistente lo indica en vez de intentar la llamada.

## Configuración
Sección `ChatbotIA` (`Endpoint`, `Modelo`, `ApiKey`). Variables: `ChatbotIA__Endpoint`, `ChatbotIA__Modelo`, `ChatbotIA__ApiKey`. Reemplaza a `IA__*`; en Azure hay que actualizar los App Settings.

Pendiente: recapturar las evidencias P11 y P12.
