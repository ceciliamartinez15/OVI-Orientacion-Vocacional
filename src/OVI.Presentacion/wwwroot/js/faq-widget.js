// Burbuja flotante de Saviox con las preguntas frecuentes. Se carga en _Layout (todas las pantallas).
(function () {
    var widget = document.getElementById('faq-widget');
    if (!widget) { return; }

    var boton = document.getElementById('fw-boton');
    var panel = document.getElementById('fw-panel');
    var cerrarBtn = document.getElementById('fw-cerrar');
    var mensajes = document.getElementById('fw-mensajes');
    var form = document.getElementById('fw-form');
    var input = document.getElementById('fw-input');
    var urlOpciones = widget.getAttribute('data-url-opciones');
    var urlPreguntar = widget.getAttribute('data-url-preguntar');

    var iniciado = false;
    var ocupado = false;
    var bloqueOpciones = null;

    // ---------- Utilidades ----------
    // Escapa HTML y habilita solo **negritas** y viñetas (las respuestas pueden venir de la BD).
    function formatear(texto) {
        var lineas = String(texto).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').split(/\r?\n/);
        var html = '', lista = false;
        lineas.forEach(function (l) {
            l = l.replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>');
            if (/^\s*[-•]\s+/.test(l)) {
                if (!lista) { html += '<ul>'; lista = true; }
                html += '<li>' + l.replace(/^\s*[-•]\s+/, '') + '</li>';
                return;
            }
            if (lista) { html += '</ul>'; lista = false; }
            if (l.trim() !== '') { html += '<p>' + l + '</p>'; }
        });
        return lista ? html + '</ul>' : html;
    }

    function bajar() { mensajes.scrollTop = mensajes.scrollHeight; }

    function agregar(remitente, texto, clasesExtra) {
        var fila = document.createElement('div');
        fila.className = 'fw-fila fw-fila-' + remitente;
        var b = document.createElement('div');
        b.className = 'fw-burbuja fw-burbuja-' + remitente + (clasesExtra ? ' ' + clasesExtra : '');
        if (remitente === 'bot') { b.innerHTML = formatear(texto); }
        else { var p = document.createElement('p'); p.textContent = texto; b.appendChild(p); }
        fila.appendChild(b);
        mensajes.appendChild(fila);
        bajar();
        return fila;
    }

    // Mantiene el bloque de opciones siempre al final de la conversación.
    function moverOpcionesAlFinal(titulo) {
        if (!bloqueOpciones) { return; }
        var t = bloqueOpciones.querySelector('.fw-opciones-titulo');
        if (t && titulo) { t.textContent = titulo; }
        mensajes.appendChild(bloqueOpciones);
        bajar();
    }

    function bloquear(si) {
        ocupado = si;
        if (bloqueOpciones) {
            bloqueOpciones.querySelectorAll('.fw-opcion').forEach(function (o) { o.disabled = si; });
        }
    }

    // ---------- Carga de opciones ----------
    function construirOpciones(lista) {
        bloqueOpciones = document.createElement('div');
        bloqueOpciones.className = 'fw-opciones';
        var titulo = document.createElement('p');
        titulo.className = 'fw-opciones-titulo';
        titulo.textContent = 'Elige una pregunta';
        bloqueOpciones.appendChild(titulo);

        lista.forEach(function (item) {
            var b = document.createElement('button');
            b.type = 'button';
            b.className = 'fw-opcion';
            b.textContent = item.pregunta;
            b.addEventListener('click', function () {
                if (ocupado) { return; }
                bloquear(true);
                agregar('usuario', item.pregunta);
                var escribiendo = agregar('bot', 'Escribiendo...', 'fw-escribiendo');
                setTimeout(function () {
                    escribiendo.remove();
                    agregar('bot', item.respuesta);
                    bloquear(false);
                    moverOpcionesAlFinal('¿Quieres saber algo más?');
                }, 350);
            });
            bloqueOpciones.appendChild(b);
        });
        mensajes.appendChild(bloqueOpciones);
        bajar();
    }

    function iniciar() {
        iniciado = true;
        agregar('bot', '¡Hola! Soy Saviox. Elige una de las preguntas frecuentes o escribe la tuya.');
        fetch(urlOpciones, { credentials: 'same-origin', headers: { 'Accept': 'application/json' } })
            .then(function (r) { if (!r.ok) { throw new Error(); } return r.json(); })
            .then(function (lista) {
                if (Array.isArray(lista) && lista.length) { construirOpciones(lista); }
                else { agregar('bot', 'Aún no hay preguntas frecuentes disponibles. Puedes escribir tu pregunta abajo.'); }
            })
            .catch(function () {
                iniciado = false; // reintenta la próxima vez que se abra
                agregar('bot', 'No pude cargar las preguntas frecuentes. Puedes escribir tu pregunta abajo.');
            });
    }

    // ---------- Abrir / cerrar ----------
    function estaAbierto() { return widget.classList.contains('fw-abierto'); }

    function abrir() {
        widget.classList.add('fw-abierto');
        boton.setAttribute('aria-expanded', 'true');
        boton.setAttribute('aria-label', 'Cerrar preguntas frecuentes');
        if (!iniciado) { mensajes.innerHTML = ''; bloqueOpciones = null; iniciar(); }
    }

    function cerrar(devolverFoco) {
        widget.classList.remove('fw-abierto');
        boton.setAttribute('aria-expanded', 'false');
        boton.setAttribute('aria-label', 'Abrir preguntas frecuentes');
        if (devolverFoco) { boton.focus(); }
    }

    boton.addEventListener('click', function () { estaAbierto() ? cerrar(false) : abrir(); });
    cerrarBtn.addEventListener('click', function () { cerrar(true); });
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && estaAbierto()) { cerrar(true); }
    });

    // ---------- Pregunta escrita ----------
    form.addEventListener('submit', function (e) {
        e.preventDefault();
        if (ocupado) { return; }
        var texto = input.value.trim();
        if (!texto) { return; }
        input.value = '';
        bloquear(true);
        agregar('usuario', texto);
        var escribiendo = agregar('bot', 'Escribiendo...', 'fw-escribiendo');

        var cabeceras = { 'Content-Type': 'application/x-www-form-urlencoded' };
        var token = form.querySelector('input[name="__RequestVerificationToken"]');
        if (token) { cabeceras['RequestVerificationToken'] = token.value; }

        fetch(urlPreguntar, {
            method: 'POST', headers: cabeceras, credentials: 'same-origin',
            body: 'mensaje=' + encodeURIComponent(texto)
        })
            .then(function (r) { if (!r.ok) { throw new Error(); } return r.json(); })
            .then(function (d) { escribiendo.remove(); agregar('bot', d.respuesta); })
            .catch(function () { escribiendo.remove(); agregar('bot', 'Ocurrió un problema de conexión. Intenta de nuevo.'); })
            .finally(function () { bloquear(false); moverOpcionesAlFinal('¿Quieres saber algo más?'); input.focus(); });
    });
})();
