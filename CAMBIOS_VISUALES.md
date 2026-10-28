# Cambios visuales (estilo VocalO / DSP1)

Solo se tocó la capa de presentación. No hay cambios en controladores, lógica, base de datos ni migraciones.

- `wwwroot/css/site.css`: paleta marfil/cafés (#FFF2E1, #A79277, #6B6056), Raleway + Roboto, botones, tarjetas, test, resultados y chat.
- `wwwroot/img/`: robot-saludando, robot-info, robot-cara y robot-pensando (comprimidos desde DSP1).
- `Views/Shared/_Layout.cshtml`: fuentes, navbar con avatar de Saviox, footer.
- `Views/Home/Index.cshtml`: hero con el robot.
- `Views/Cuenta/Login.cshtml` y `Registro.cshtml`: dos columnas con la mascota.
- `Views/Test/Index.cshtml`: pantalla de introducción y una pregunta por paso (mismo formulario Likert 1–5, mismos nombres de campos).
- `Views/Test/Resultado.cshtml`: aviso "Nota importante" y tarjetas de recomendación.
- Chat: reemplazado por el chatbot de DSP1, ver `CAMBIOS_CHATBOT.md`.

Pendiente tras aplicar: recapturar docs/evidencias, actualizar manual de usuario y grabar el video con la UI nueva.
