// Chat genérico reutilizado por Views/Chatbot/Faq.cshtml y Views/Chatbot/Contextual.cshtml.
// Cada vista solo pasa el id del form, del input, del contenedor de mensajes y la URL del endpoint.
function initChatOvi(opciones) {
    var form = document.getElementById(opciones.formId);
    var input = document.getElementById(opciones.inputId);
    var contenedorMensajes = document.getElementById(opciones.mensajesId);
    var endpointUrl = opciones.endpointUrl;

    var enviando = false;

    function formatearHora() {
        var ahora = new Date();
        var horas = ahora.getHours().toString().padStart(2, '0');
        var minutos = ahora.getMinutes().toString().padStart(2, '0');
        return horas + ':' + minutos;
    }

    // Convierte el texto plano de la respuesta del bot a un HTML mínimo y seguro:
    // escapa cualquier etiqueta primero y solo después habilita **negritas** y
    // viñetas (líneas que empiezan con "-" o "•"), agrupándolas en <ul><li>.
    function formatearTextoBot(texto) {
        var escapado = texto
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');

        var lineas = escapado.split(/\r?\n/);
        var html = '';
        var dentroDeLista = false;

        lineas.forEach(function (lineaOriginal) {
            var linea = lineaOriginal.replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>');
            var esViñeta = /^\s*[-•]\s+/.test(linea);

            if (esViñeta) {
                if (!dentroDeLista) {
                    html += '<ul>';
                    dentroDeLista = true;
                }
                html += '<li>' + linea.replace(/^\s*[-•]\s+/, '') + '</li>';
                return;
            }

            if (dentroDeLista) {
                html += '</ul>';
                dentroDeLista = false;
            }

            if (linea.trim() !== '') {
                html += '<p>' + linea + '</p>';
            }
        });

        if (dentroDeLista) {
            html += '</ul>';
        }

        return html;
    }

    function agregarBurbuja(remitente, texto) {
        var fila = document.createElement('div');
        fila.className = 'chat-fila chat-fila-' + remitente;

        var burbuja = document.createElement('div');
        burbuja.className = 'chat-burbuja chat-burbuja-' + remitente;

        var contenidoTexto = document.createElement('div');
        contenidoTexto.className = 'chat-burbuja-texto';

        if (remitente === 'bot') {
            contenidoTexto.innerHTML = formatearTextoBot(texto);
        } else {
            var parrafo = document.createElement('p');
            parrafo.textContent = texto;
            contenidoTexto.appendChild(parrafo);
        }

        burbuja.appendChild(contenidoTexto);
        fila.appendChild(burbuja);

        var hora = document.createElement('span');
        hora.className = 'chat-hora';
        hora.textContent = formatearHora();
        fila.appendChild(hora);

        contenedorMensajes.appendChild(fila);
        contenedorMensajes.scrollTop = contenedorMensajes.scrollHeight;

        return fila;
    }

    function mostrarEscribiendo() {
        var fila = document.createElement('div');
        fila.className = 'chat-fila chat-fila-bot';
        fila.id = 'chat-indicador-escribiendo';

        var burbuja = document.createElement('div');
        burbuja.className = 'chat-burbuja chat-burbuja-bot chat-escribiendo';
        burbuja.innerHTML = '<div class="chat-burbuja-texto"><p>Escribiendo...</p></div>';

        fila.appendChild(burbuja);
        contenedorMensajes.appendChild(fila);
        contenedorMensajes.scrollTop = contenedorMensajes.scrollHeight;
    }

    function ocultarEscribiendo() {
        var indicador = document.getElementById('chat-indicador-escribiendo');
        if (indicador) {
            indicador.remove();
        }
    }

    function enviarMensaje(mensaje) {
        agregarBurbuja('usuario', mensaje);
        mostrarEscribiendo();

        var cabeceras = { 'Content-Type': 'application/x-www-form-urlencoded' };
        var campoToken = form.querySelector('input[name="__RequestVerificationToken"]');
        if (campoToken) {
            cabeceras['RequestVerificationToken'] = campoToken.value;
        }

        fetch(endpointUrl, {
            method: 'POST',
            headers: cabeceras,
            credentials: 'same-origin',
            body: 'mensaje=' + encodeURIComponent(mensaje)
        })
            .then(function (resp) {
                if (!resp.ok) {
                    throw new Error('Respuesta HTTP no exitosa');
                }
                return resp.json();
            })
            .then(function (datos) {
                ocultarEscribiendo();
                agregarBurbuja('bot', datos.respuesta);
            })
            .catch(function () {
                ocultarEscribiendo();
                agregarBurbuja('bot', 'Ocurrió un problema de conexión. Intenta de nuevo.');
            })
            .finally(function () {
                enviando = false;
                input.disabled = false;
                input.focus();
            });
    }

    form.addEventListener('submit', function (evento) {
        evento.preventDefault();

        if (enviando) {
            return;
        }

        var mensaje = input.value.trim();
        if (mensaje === '') {
            return;
        }

        enviando = true;

        // No deshabilitamos el input dentro de este mismo handler de submit: si este
        // formulario alguna vez se envía de forma nativa, deshabilitar el campo antes de
        // que el navegador termine de construir el envío puede hacer que su valor no
        // viaje. Lo deshabilitamos en un setTimeout(0), después de este ciclo de eventos.
        setTimeout(function () {
            input.disabled = true;
        }, 0);

        input.value = '';
        enviarMensaje(mensaje);
    });

    // Botones de sugerencia rápida (si la vista los tiene): auto-envían esa pregunta.
    document.querySelectorAll('[data-chat-sugerencia]').forEach(function (boton) {
        boton.addEventListener('click', function () {
            if (enviando) {
                return;
            }
            input.value = boton.getAttribute('data-chat-sugerencia');
            form.requestSubmit ? form.requestSubmit() : form.dispatchEvent(new Event('submit', { cancelable: true }));
        });
    });

    input.focus();
}
