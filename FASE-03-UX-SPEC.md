# FASE 3 — ESPECIFICACIÓN DE UX / UI (ux-spec.md)

> **Proyecto:** Plataforma ciudadana de cruce de información sobre personas desaparecidas — República Dominicana
> **Versión:** 1.0-draft
> **Propósito:** Mapa de pantallas, diseño de wireframes, sistema de diseño y accesibilidad.

---

## 1. MAPA DE PANTALLAS

### 1.1 Árbol de navegación

```
Landing (pública)
├── / (index)
├── /buscar ................... Buscador público de personas
├── /persona/{id} ............ Detalle público de persona
├── /reportar ................ Formulario de reporte (paso a paso)
├── /reportar/exito .......... Confirmación de reporte
├── /seguimiento ............. Ingresar código de seguimiento
├── /seguimiento/{codigo} .... Estado del reporte
├── /localizada .............. Reportar que alguien está a salvo
├── /mapa ................... Mapa de casos activos
├── /sobre .................. Sobre el proyecto
├── /contacto ............... Contacto y autoridades
│
├── /auth/login .............. Inicio de sesión
├── /auth/register ........... Registro de usuario
│
├── /panel/ .................. Panel autenticado (según rol)
│   ├── (familiar) dashboard .. Mis reportes
│   ├── (verificador) cola .... Coincidencias pendientes
│   ├── (verificador) match ... Revisión de match
│   ├── (salud) ingresar ...... Registro de paciente NN
│   ├── (salud) matches ....... Posibles coincidencias
│   ├── (admin) dashboard ..... Métricas del sistema
│   ├── (admin) fuentes ....... Estado de fuentes de datos
│   ├── (admin) usuarios ...... Gestión de usuarios
│   ├── (superadmin) config ... Configuración del sistema
│   └── (superadmin) auditoria. Log de auditoría
```

### 1.2 Prioridad de implementación

| Pantalla | Prioridad | Frecuencia de uso esperada | Módulo |
|---|---|---|---|
| Landing institucional | **P0** | Muy alta (puerta de entrada) | 12 |
| Reportar persona | **P0** | Alta | 1 |
| Confirmación reporte | **P0** | Alta | 1 |
| Buscador público | **P0** | Muy alta | 6 |
| Detalle de persona | **P0** | Alta | 6 |
| Reportar localizada | **P0** | Media | 2 |
| Seguimiento (código) | **P0** | Media | 1 |
| Login/Registro | **P1** | Media | — |
| Panel verificador (cola) | **P1** | Alta (uso interno) | 5 |
| Panel verificador (match) | **P1** | Alta (uso interno) | 5 |
| Dashboard admin | **P1** | Media (uso interno) | 8 |
| Estado fuentes | **P1** | Baja (uso interno) | 8 |
| Ingreso paciente NN | **P1** | Baja (uso interno) | 2 |
| Mapa de casos | **P2** | Media | 7 |
| Panel familiar | **P2** | Baja | 1 |
| Gestión usuarios | **P2** | Baja (uso interno) | — |
| Config sistema | **P2** | Muy baja (uso interno) | 4 |
| Auditoría | **P2** | Muy baja (uso interno) | 10 |

---

## 2. WIREFRAMES DESCRITOS (PANTALLAS CRÍTICAS P0)

### 2.1 Landing institucional `/`

#### Jerarquía visual

```
┌──────────────────────────────────────────────────────────────┐
│  [Logo]   Buscar persona   Reportar   Sobre el proyecto       │
│  ──────────────────────────────────────────────────────────── │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐     │
│  │  🧡  ¿Buscas a alguien o quieres reportar?          │     │
│  │                                                     │     │
│  │  [🔍 Buscar persona desaparecida]                   │     │
│  │  [📢 Reportar una desaparición]                     │     │
│  │                                                     │     │
│  │  Si es una emergencia, llama al 911                 │     │
│  └─────────────────────────────────────────────────────┘     │
│                                                               │
│  ── Alertas activas ────────────────────────────────────────── │
│  [🟠 Alerta Amber · Juan Pérez, 7 años · Sto. Dgo.]          │
│  [🟠 Alerta Rosa · María García, 29 años · Santiago]          │
│  Ver todas →                                                  │
│                                                               │
│  ── ¿Cómo funciona? ────────────────────────────────────────── │
│  1. Reportas o buscas       2. Cruzamos datos                 │
│  3. Verificamos              4. Cerramos el caso              │
│                                                               │
│  ── Estadísticas ───────────────────────────────────────────── │
│  🟢 245 casos activos    ✅ 189 personas localizadas          │
│                                                               │
│  ── Transparencia ──────────────────────────────────────────── │
│  🟡 Esta plataforma es COMPLEMENTARIA. No sustituye           │
│  a la Policía Nacional ni al 911.                             │
│  En caso de emergencia llama al 911.                          │
│                                                               │
│  ── Footer ─────────────────────────────────────────────────── │
│  Policía Nacional | 911 | Alertas RD | Asodofade               │
│  [Ley 172-13 protección de datos] [Términos de uso]           │
└──────────────────────────────────────────────────────────────┘
```

#### Estados

| Estado | Comportamiento |
|---|---|
| **Normal** | Muestra estadísticas reales + alertas activas |
| **Cargando** | Skeleton loader en tarjetas de alertas y estadísticas |
| **Error (API caída)** | Banner: "Algunas secciones no están disponibles. Los reportes de emergencia continúan funcionando." |
| **Vacío (sin alertas)** | Texto: "No hay alertas activas en este momento." |

#### Copy exacto (español dominicano)

- Título hero: **"¿Buscas a alguien o quieres reportar?"**
- Botón primario: **"Buscar persona desaparecida"** (icono lupa)
- Botón secundario: **"Reportar una desaparición"** (icono megáfono)
- Nota legal hero: **"Si es una emergencia, llama al 911"** (texto pequeño, color de advertencia)
- Sección alertas: **"Alertas activas"**
- Sección pasos: **"¿Cómo funciona?"** → **"1. Reportas o buscas — 2. Cruzamos datos — 3. Verificamos — 4. Cerramos el caso"**
- Sección transparencia: **"Esta plataforma es COMPLEMENTARIA. No sustituye a la Policía Nacional ni al 911. En caso de emergencia llama al 911."**

---

### 2.2 Reportar persona desaparecida `/reportar`

#### Flujo (step-by-step, una pregunta por paso)

```
PASO 1/6 — ¿A quién buscas?
┌──────────────────────────────────────────────┐
│  ¿A quién buscas?                            │
│                                              │
│  [Foto: subir foto] (opcional)               │
│                                              │
│  Primer nombre         [________________]    │
│  Segundo nombre        [________________]    │
│  Primer apellido       [________________]    │
│  Segundo apellido      [________________]    │
│  Alias/apodo           [________________]    │
│                                              │
│  [Siguiente →]                               │
│                                              │
│  Paso 1 de 6  ● ●○○○○○                       │
└──────────────────────────────────────────────┘

PASO 2/6 — ¿Qué edad tiene?
┌──────────────────────────────────────────────┐
│  ¿Qué edad tiene o cuándo nació?             │
│                                              │
│  ◉ Aproximadamente _______ años              │
│  ○ Fecha de nacimiento [__/__/____]          │
│                                              │
│  Sexo:  ○ Masculino  ○ Femenino  ○ Otro     │
│                                              │
│  [← Atrás]  [Siguiente →]                   │
│                                              │
│  Paso 2 de 6  ●●○○○○○                       │
└──────────────────────────────────────────────┘

PASO 3/6 — ¿Dónde se le vio por última vez?
┌──────────────────────────────────────────────┐
│  ¿Dónde se le vio por última vez?            │
│                                              │
│  Escribe el lugar:                           │
│  [________________________________]          │
│  Ej: "Av. 27 de Febrero, frente a Acrópolis" │
│                                              │
│  Provincia:  [Seleccionar ▼]                 │
│  Municipio:  [Seleccionar ▼]                 │
│                                              │
│  Fecha:      [__/__/____]                    │
│  Hora aprox: [__:__]                         │
│                                              │
│  [← Atrás]  [Siguiente →]                   │
│                                              │
│  Paso 3 de 6  ●●●○○○○                       │
└──────────────────────────────────────────────┘

PASO 4/6 — ¿Cómo iba vestido/a? (opcional)
┌──────────────────────────────────────────────┐
│  ¿Cómo iba vestido/a? (opcional)             │
│                                              │
│  Vestimenta:                                  │
│  [________________________________]          │
│  Ej: "Camisa azul, jeans, tenis blancos"     │
│                                              │
│  Señales particulares (tatuajes, cicatrices): │
│  [________________________________]          │
│                                              │
│  ¿Tiene alguna condición médica?              │
│  [________________________________]          │
│  (Esto solo lo verán las autoridades) 🔒     │
│                                              │
│  [← Atrás]  [Siguiente →]                   │
│                                              │
│  Paso 4 de 6  ●●●●○○○                       │
└──────────────────────────────────────────────┘

PASO 5/6 — Tus datos de contacto
┌──────────────────────────────────────────────┐
│  Tus datos de contacto                       │
│  (Solo para que podamos notificarte)         │
│                                              │
│  Tu nombre:      [________________]          │
│  Relación:       [Seleccionar ▼]             │
│  Teléfono:       [809-___-____]              │
│  Correo:         [________________]          │
│                                              │
│  ¿Quieres recibir notificaciones?            │
│  ○ SMS   ○ Correo   ○ Ambos                 │
│                                              │
│  [← Atrás]  [Siguiente →]                   │
│                                              │
│  Paso 5 de 6  ●●●●●○○                       │
└──────────────────────────────────────────────┘

PASO 6/6 — Revisa y confirma
┌──────────────────────────────────────────────┐
│  Revisa que todo esté correcto               │
│                                              │
│  [foto]  Juan Carlos Pérez Martínez          │
│          36 años, Masculino                  │
│          Desapareció el 15/06/2026           │
│          Última vez: Av. 27 de Febrero       │
│                                              │
│  [✅ Acepto términos y condiciones]          │
│  [✅ Entiendo que esto no reemplaza          │
│       una denuncia policial]                 │
│                                              │
│  [Enviar reporte]                            │
│                                              │
│  Paso 6 de 6  ●●●●●●                        │
└──────────────────────────────────────────────┘
```

#### Estados del formulario

| Estado | Comportamiento |
|---|---|
| **Carga inicial** | Skeleton de 6 placeholders circulares (los steps) |
| **Paso activo** | Animación suave de entrada, transición fade |
| **Error de validación** | Campo en rojo + texto: "Por favor completa este campo" / "El teléfono no es válido" |
| **Error de red (al enviar)** | Banner: "Hubo un error al enviar tu reporte. No se ha perdido tu información. Intenta de nuevo." Los datos se mantienen en el formulario. |
| **Éxito** | Pantalla de confirmación completa (ver abajo) |

#### Pantalla de éxito `/reportar/exito`

```
┌──────────────────────────────────────────────┐
│                                              │
│    ✅ Reporte recibido                       │
│                                              │
│    Hemos recibido tu reporte.                │
│                                              │
│    Tu código de seguimiento es:              │
│    ┌──────────────────────────┐              │
│    │     LP-7F3K2             │              │
│    │     [Copiar código]      │              │
│    └──────────────────────────┘              │
│                                              │
│    📱 Te enviaremos un SMS cuando             │
│    haya novedades.                           │
│                                              │
│    Mientras tanto:                           │
│    • Si encuentras a la persona,              │
│      [márcala como localizada]               │
│    • También puedes ver el estado             │
│      [Dar seguimiento]                       │
│                                              │
│    Recuerda: si es una emergencia,            │
│    llama al 911.                             │
│                                              │
│    [Volver al inicio]                        │
└──────────────────────────────────────────────┘
```

---

### 2.3 Buscador público `/buscar`

```
┌──────────────────────────────────────────────┐
│  🔍 Buscar persona desaparecida               │
│                                              │
│  [___________________________] [Buscar]      │
│                                              │
│  Filtros:  [Nombre ▼] [Edad ▼] [Provincia ▼] │
│           [Estado ▼] [Fecha ▼]               │
│                                              │
│  ── Resultados: 12 encontrados ──            │
│                                              │
│  ┌────────────────────────────────────────┐  │
│  │ [foto] Juan Pérez                      │  │
│  │        36 años · Santo Domingo         │  │
│  │        Desapareció: 15/06/2026         │  │
│  │        🟠 Alerta activa                │  │
│  │        [Ver detalle →]                 │  │
│  ├────────────────────────────────────────┤  │
│  │ [foto] María García                    │  │
│  │        29 años · Santiago              │  │
│  │        Desapareció: 10/06/2026         │  │
│  │        🟢 Localizada                   │  │
│  │        [Ver detalle →]                 │  │
│  └────────────────────────────────────────┘  │
│                                              │
│  [< 1 2 3 ... 10 >]                          │
└──────────────────────────────────────────────┘
```

#### Estados

| Estado | Comportamiento |
|---|---|
| **Normal** | Resultados paginados con foto + datos clave + badge de estado |
| **Cargando** | Spinner + "Buscando..." |
| **Vacío (sin resultados)** | "No encontramos personas con esos datos. ¿Quieres intentar con otros filtros? También puedes [crear un reporte]." |
| **Error** | "Hubo un error en la búsqueda. Por favor intenta de nuevo." |
| **Sin conexión** | Banner: "Parece que no tienes conexión a internet. Los resultados pueden no estar actualizados." |

#### Filtros detallados

| Filtro | Tipo | Opciones |
|---|---|---|
| Nombre | Texto libre | Búsqueda parcial (LIKE) |
| Edad desde / hasta | Rango numérico | 0-120 |
| Provincia | Dropdown | 32 provincias + DN |
| Estado caso | Dropdown | Todos / Desaparecido / Localizado |
| Fecha desaparición | Rango fechas | Desde / Hasta |
| Sexo | Dropdown | Todos / Masculino / Femenino |
| Tipo alerta | Dropdown | Todas / Amber / Silver / Azul / Rosa |

---

### 2.4 Detalle de persona `/persona/{id}`

```
┌──────────────────────────────────────────────┐
│                                              │
│  [foto grande]                               │
│                                              │
│  Juan Carlos Pérez Martínez                  │
│  36 años · Masculino                         │
│                                              │
│  🟠 DESAPARECIDO · Alerta Amber              │
│                                              │
│  ── Datos generales ──                       │
│  Fecha desaparición: 15 de junio de 2026     │
│  Última vez vista: Av. 27 de Febrero,        │
│   Santo Domingo                               │
│  Vestimenta: Camisa azul, jeans, tenis       │
│  Señales: Tatuaje de cruz en brazo derecho   │
│                                              │
│  ── ¿Tienes información? ──                  │
│  Si has visto a esta persona o sabes         │
│  algo, por favor repórtalo:                  │
│                                              │
│  [Tengo información]                         │
│                                              │
│  ── Línea de tiempo ──                       │
│  🕐 15/06 — Reportado                        │
│  🕐 15/06 — Alerta activada                  │
│  🕐 16/06 — Coincidencia detectada           │
│             (hospital regional)              │
│             [Pendiente de verificación]      │
│                                              │
│  ── Instituciones ──                         │
│  Si tienes información, contacta:            │
│  Policía Nacional: 809-555-0100              │
│  Alertas RD: *123#                          │
│                                              │
│  Reporta información anónima: [link]         │
│                                              │
│  [← Volver a resultados]                     │
└──────────────────────────────────────────────┘
```

#### Estado "Localizado"

```
┌──────────────────────────────────────────────┐
│                                              │
│  ✅ LOCALIZADA · Caso cerrado                │
│                                              │
│  María García fue localizada el              │
│  18 de junio de 2026                         │
│                                              │
│  Está a salvo.                               │
│                                              │
│  Gracias a todos los que compartieron        │
│  información.                                │
│                                              │
│  Si eres familiar y no has sido notificado,  │
│  contacta a la Policía Nacional.             │
│                                              │
│  [Volver al inicio]                          │
└──────────────────────────────────────────────┘
```

### 2.5 Reportar persona localizada `/localizada`

```
┌──────────────────────────────────────────────┐
│  ✅ ¿Ya encontraste a la persona?            │
│                                              │
│  Cuéntanos para cerrar el reporte y          │
│  avisar a quienes la están buscando.         │
│                                              │
│  Código de seguimiento (si lo tienes):       │
│  [LP-_________]                              │
│                                              │
│  ── O busca por nombre ──                    │
│  Nombre de la persona:                       │
│  [________________________________]          │
│                                              │
│  Tu nombre:      [________________]          │
│  Tu teléfono:    [809-___-____]              │
│  ¿Eres familiar?  ○ Sí  ○ No                │
│                                              │
│  ¿Dónde fue localizada?                      │
│  [________________________________]          │
│                                              │
│  ¿Está a salvo?  ○ Sí  ○ No                 │
│                                              │
│  [Enviar reporte de localización]            │
│                                              │
│  ⚠️ Importante: un verificador confirmará   │
│  esta información antes de cerrar el caso.   │
└──────────────────────────────────────────────┘
```

### 2.6 Dashboard de verificador `/panel/verificador` (P1)

```
┌──────────────────────────────────────────────┐
│  Panel de verificación        [Tu nombre] ▼  │
│  ──────────────────────────────────────────── │
│                                              │
│  🔔 Coincidencias pendientes: 7               │
│                                              │
│  ── Prioritarias (score > 85%) ──            │
│  ┌────────────────────────────────────────┐  │
│  │ 🔴 Score: 92% · Juan Pérez vs.         │  │
│  │     Hospital Regional Santiago         │  │
│  │     Reportado: 15/06 · Ingresado: 15/06│  │
│  │     [Revisar →]                        │  │
│  ├────────────────────────────────────────┤  │
│  │ 🟡 Score: 78% · María García vs.       │  │
│  │     Hospital Municipal La Vega         │  │
│  │     Reportado: 10/06 · Ingresado: 13/06│  │
│  │     [Revisar →]                        │  │
│  └────────────────────────────────────────┘  │
│                                              │
│  ── Revisión de match ──                     │
│  ┌────────────────────────────────────────┐  │
│  │ [Comparación lado a lado]              │  │
│  │                                        │  │
│  │ REPORTE             │ REGISTRO          │  │
│  │ Juan Pérez          │ Juan C. Pérez     │  │
│  │ 36 años             │ 35-40 aprox       │  │
│  │ Masculino           │ Masculino         │  │
│  │ Sto. Dgo.           │ Santiago          │  │
│  │ Tatuaje brazo der.  │ Tatuaje brazo der.│  │
│  │ [foto]              │ [foto hospital]   │  │
│  │                     │                   │  │
│  │ [✅ Coinciden — confirmar]              │  │
│  │ [❌ No coinciden — descartar]           │  │
│  │ [🤷 Más datos necesarios]              │  │
│  └────────────────────────────────────────┘  │
└──────────────────────────────────────────────┘
```

---

## 3. SISTEMA DE DISEÑO TAILWIND

### 3.1 Paleta de colores

```css
/* 🌅 Tonos principales — cercanía, confianza, urgencia controlada */

--color-primary:        #1B4F72;    /* Azul profundo — confianza, institucional */
--color-primary-light:  #3498DB;    /* Azul medio — hover, links */
--color-primary-dark:   #0E2E45;    /* Azul oscuro — headers, footer */

--color-secondary:      #E67E22;    /* Naranja — alertas, urgencia, sin ansiedad */
--color-secondary-light:#F39C12;    /* Ámbar — badges de alerta */

--color-accent:         #27AE60;    /* Verde — localizado, éxito, esperanza */
--color-accent-light:   #2ECC71;    /* Verde claro — badges de localizado */

--color-danger:         #C0392B;    /* Rojo — errores graves, peligro */
--color-danger-light:   #E74C3C;    /* Rojo claro — advertencias */

--color-neutral-50:     #F8F9FA;   /* Fondo general */
--color-neutral-100:    #E9ECEF;   /* Bordes suaves */
--color-neutral-200:    #DEE2E6;   /* Bordes */
--color-neutral-300:    #CED4DA;   /* Borde inputs */
--color-neutral-400:    #ADB5BD;   /* Texto placeholder */
--color-neutral-500:    #6C757D;   /* Texto secundario */
--color-neutral-600:    #495057;   /* Texto cuerpo */
--color-neutral-700:    #343A40;   /* Texto headings */
--color-neutral-800:    #212529;   /* Texto principal */

--color-alert-amber:    #E67E22;   /* Alerta Amber (niños) */
--color-alert-silver:   #95A5A6;   /* Alerta Silver (adultos mayores) */
--color-alert-azul:     #3498DB;   /* Alerta Azul (discapacidad) */
--color-alert-rosa:     #E91E63;   /* Alerta Rosa (violencia género) */
```

### 3.2 Principios de uso del color

| Principio | Regla |
|---|---|
| **No usar rojo como color principal** | El rojo solo para errores graves. Nunca para botones principales. |
| **Verde = esperanza** | Usar solo para "localizado" o "éxito". No para "confirmar" acciones destructivas. |
| **Naranja = alerta** | Tono cálido que comunica urgencia sin pánico (a diferencia del rojo). |
| **Alto contrate** | Texto siempre ≥ #495057 sobre fondo blanco. Nunca gris claro sobre blanco. |
| **Fondo siempre blanco o casi blanco** | No usar fondos oscuros. Los usuarios en crisis necesitan claridad visual. |
| **Alertas tipificadas por color** | Amber = naranja, Silver = gris, Azul = azul, Rosa = rosado. |

### 3.3 Tipografía

| Propiedad | Valor |
|---|---|
| **Familia principal** | `Inter` (sans-serif,干净 moderna, buena legibilidad en pantalla) |
| **Familia secundaria (títulos)** | `Inter` (misma familia — consistencia) |
| **Fallback** | `system-ui, -apple-system, sans-serif` |
| **Tamaño base** | `16px` |
| **Escala** | `12, 14, 16, 18, 20, 24, 30, 36, 48` |
| **Line-height** | `1.5` (cuerpo), `1.2` (títulos) |
| **Peso títulos** | `700` (bold) |
| **Peso cuerpo** | `400` (regular) |
| **Peso énfasis** | `600` (semibold) |

### 3.4 Componentes reutilizables

#### Botones

```
[🔍 Buscar persona]          ← Botón primario (bg-primary, white text)
[📢 Reportar]                 ← Botón secundario (outline primary)
[✅ Localizada]                ← Botón éxito (bg-accent, white)
[⚠️ No coinciden]             ← Botón peligro (outline danger)
[⬅ Volver]                    ← Botón ghost (sin fondo)
```

- **Altura**: 48px (móvil), 44px (desktop)
- **Border radius**: 8px
- **Padding horizontal**: 24px
- **Estados**: hover, active, disabled, focus (ring 2px)
- **Tamaño touch**: mínimo 44x44px (WCAG target size)

#### Tarjetas de persona (card)

```
┌──────────────────────┐
│ [foto 80x80px]       │
│ Nombre completo       │ ← bold 16px
│ Edad · Provincia      │ ← text-neutral-500 14px
│ 🟠 Desaparecido       │ ← badge con color según estado
│ [Ver detalle]         │ ← link
└──────────────────────┘
```

- **Border**: 1px solid neutral-200
- **Border radius**: 12px
- **Padding**: 16px
- **Hover**: shadow-sm + border-primary-light
- **Gap entre cards**: 16px
- **Responsive**: 1 col (móvil), 2 cols (tablet), 3 cols (desktop)

#### Badges de estado

| Estado | Color fondo | Color texto | Texto |
|---|---|---|---|
| Recibido | `bg-blue-50` | `text-blue-700` | "Recibido" |
| En verificación | `bg-yellow-50` | `text-yellow-700` | "En verificación" |
| Coincidencia detectada | `bg-orange-50` | `text-orange-700` | "Posible coincidencia" |
| En investigación | `bg-purple-50` | `text-purple-700` | "En investigación" |
| Localizado con vida | `bg-green-50` | `text-green-700` | "✅ Localizado" |
| Localizado fallecido | `bg-gray-50` | `text-gray-600` | "Localizado (fallecido)" |
| Cerrado sin resolver | `bg-red-50` | `text-red-700` | "Cerrado" |

#### Formulario (input)

```
[________________________]  ← altura 48px, border neutral-300, radius 8px
                            ← focus: ring 2px primary, border primary
                            ← error: border danger, mensaje rojo abajo
                            ← placeholder: text-neutral-400
```

- **Labels**: arriba del input, weight 600, 14px
- **Error text**: 12px, color danger, margin-top 4px
- **Hint text**: 12px, color neutral-500, margin-top 4px

#### Skeleton loader

```
┌──────────────────────┐
│ ░░░░░░░░░░░░░░░░░░░░ │ ← animate-pulse bg-neutral-200
│ ░░░░░░░░░░           │
│ ░░░░░░░░░░░░░░       │
└──────────────────────┘
```

### 3.5 Responsive breakpoints

| Breakpoint | Ancho | Comportamiento |
|---|---|---|
| **Móvil** | < 640px | 1 columna, navegación en hamburguesa, botones full-width |
| **Tablet** | 640-1024px | 2 columnas, navegación visible parcial |
| **Desktop** | > 1024px | 3 columnas, navegación completa, sidebar en paneles internos |

---

## 4. ACCESIBILIDAD WCAG AA

### 4.1 Checklist por nivel

| Criterio WCAG | Estado | Implementación |
|---|---|---|
| **1.1.1 Texto alternativo** | ✅ | Todas las imágenes decorativas con `alt=""`, fotos de personas con `alt="Foto de [nombre]"` |
| **1.2.1 Contenido solo audio/video** | N/A | No hay multimedia en MVP |
| **1.3.1 Info y relaciones** | ✅ | Encabezados jerárquicos h1→h2→h3, listas con `<ul>`, tablas con `<th>` |
| **1.3.2 Secuencia significativa** | ✅ | Orden DOM = orden visual |
| **1.4.1 Uso del color** | ✅ | Todos los badges de estado incluyen icono + texto, no solo color |
| **1.4.3 Contraste mínimo** | ✅ | Ratio ≥ 4.5:1 texto normal, ≥ 3:1 texto grande. Verificado contra paleta. |
| **1.4.4 Cambio de tamaño texto** | ✅ | Unidades rem, no px. Zoom 200% sin pérdida de contenido. |
| **1.4.10 Reflow** | ✅ | Sin scroll horizontal en 320px×256px viewport |
| **1.4.11 Contraste no textual** | ✅ | Componentes UI (bordes inputs, focus ring) ≥ 3:1 |
| **2.1.1 Teclado** | ✅ | Todos los elementos interactivos accesibles por teclado (Tab, Enter, Space) |
| **2.1.2 Sin trampa de teclado** | ✅ | Modales: Escape cierra, Tab循環 dentro del modal |
| **2.4.1 Saltar navegación** | ✅ | Skip link "Saltar al contenido principal" al inicio de cada página |
| **2.4.3 Orden del foco** | ✅ | Foco sigue orden lógico de lectura |
| **2.4.4 Propósito del enlace** | ✅ | Links con texto descriptivo, no "click aquí" |
| **2.4.7 Foco visible** | ✅ | focus-visible con ring 2px de color primary |
| **3.2.1 Al recibir el foco** | ✅ | Ningún cambio de contexto al hacer focus |
| **3.2.2 Al recibir entrada** | ✅ | Botones de envío no se ejecutan en onchange |
| **3.3.1 Identificación de errores** | ✅ | Errores descritos textualmente, asociados al campo con aria-describedby |
| **3.3.2 Etiquetas o instrucciones** | ✅ | Todos los inputs con `<label>`, placeholder es complementario |
| **4.1.2 Nombre, rol, valor** | ✅ | ARIA roles en elementos personalizados |
| **4.1.3 Mensajes de estado** | ✅ | `role="alert"` en errores, `aria-live="polite"` en actualizaciones |

### 4.2 Consideraciones específicas para usuarios en crisis

| Necesidad | Solución |
|---|---|
| **Ansiedad por tiempos de carga** | Skeleton inmediato (<200ms), mensaje: "Estamos cargando la información" |
| **Temblor / dificultad motriz** | Target mínimo 44×44px, sin gestos complejos (swipe), botones grandes |
| **Lectura apresurada** | Jerarquía visual clara: una pregunta por paso, sin párrafos largos |
| **Estrés emocional al leer "fallecido"** | Usar "Localizado (fallecido)" en lugar de "Muerto". En notificaciones: "Tenemos información sobre [nombre]. Te contactaremos." |
| **Conexión débil (2G / zona rural)** | Peso total de página <500KB. Imágenes lazy loading + compresión WebP. Offline banner. |
| **Poco familiarizado con tecnología** | Instrucciones en lenguaje simple, iconos reconocibles, flujo lineal sin ramificaciones |

### 4.3 Rendimiento en conexión débil

| Estrategia | Detalle |
|---|---|
| **Tamaño de página < 500KB** | Tailwind compilado (purge CSS), JS mínimo (sin framework pesado), imágenes redimensionadas |
| **Imágenes responsivas** | `srcset` con 3 tamaños (480px, 800px, 1200px), formato WebP con fallback JPG |
| **Lazy loading** | Imágenes de resultados de búsqueda con `loading="lazy"` |
| **Offline detection** | Banner: "Parece que tienes conexión lenta. Los datos pueden tardar en cargar." |
| **Prioridad de carga** | CSS crítico inline, JS diferido, analytics después de paint |
| **Service Worker (futuro)** | Cachear landing page y buscador para consulta offline parcial |

---

*Fin de la FASE 3. Pendiente de validación del usuario antes de continuar a FASE 4.*
