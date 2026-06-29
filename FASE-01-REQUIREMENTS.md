# FASE 1 — ESPECIFICACIÓN FUNCIONAL (requirements.md)

> **Proyecto:** Plataforma ciudadana de cruce de información sobre personas desaparecidas — República Dominicana
> **Versión:** 1.0-draft
> **Propósito:** Documento de especificación funcional para validación del alcance antes de pasar a diseño técnico.

---

## 1. PROBLEMA A RESOLVER

### 1.1 Statement

> **"En República Dominicana, la información sobre una persona desaparecida vive fragmentada entre la Policía Nacional, hospitales públicos, la Procuraduría, medios de comunicación, redes sociales y la memoria de los familiares. No existe un punto único, accesible al ciudadano y actualizado en tiempo casi real, donde cualquier persona pueda (a) reportar una desaparición, (b) verificar si alguien ha sido localizado, o (c) crucer datos — voluntaria y colaborativamente — para acelerar la resolución de casos."**

### 1.2 Impacto medible esperado

| Indicador | Línea base | Meta MVP (6 meses) | Meta plataforma completa (2 años) |
|---|---|---|---|
| Tiempo entre reporte ciudadano y primera activación de búsqueda coordinada | >24-48h (estudios de Asodofade) | Reducción a <6h | Reducción a <2h |
| Casos con al menos un cruce de datos (ciudadano + fuente oficial) | ~0% (no existe sistema) | 30% de reportes activos | 80% de reportes activos |
| Tasa de resolución con vida | ~80% actual (est. Policía: 2,651/3,313) | Mantener o mejorar | >90% |
| Reportes falsos detectados antes de publicación | N/A (sistema nuevo) | >90% capturados automática o semiautomáticamente | >95% |
| Fuentes de datos externas integradas | 0 | 2-3 fuentes (Policía + 1 hospital regional + datos.gob.do) | 10+ fuentes (todas las regiones de salud + Procuraduría + 911) |

### 1.3 Alineación con política pública existente

La **Ley 25-26 "Alertas RD"** (promulgada junio 2026) crea el Registro Nacional de Personas Desaparecidas y establece el marco legal. Esta plataforma **no reemplaza ni duplica** dicho registro, sino que actúa como:
- **Capa ciudadana** de reporte temprano (antes de que la persona acuda a una unidad policial)
- **Capa de cruce colaborativo** (voluntarios, familiares y personal de salud cruzando información)
- **Alimentador** del Registro Nacional: datos verificados pueden ser canalizados a la Policía Nacional
- **Capa de transparencia** para que los familiares puedan dar seguimiento al estado de un caso sin tener que llamar a 4 instituciones distintas

---

## 2. ACTORES DEL SISTEMA

| # | Actor | Descripción | Tipificación | ¿Requiere registro? |
|---|---|---|---|---|
| A01 | **Ciudadano reportante** | Persona que reporta una desaparición (familiar, amigo, vecino, testigo) | No autenticado (reporte inicial) / Autenticado (seguimiento) | Mínimo: correo/teléfono para confirmación |
| A02 | **Familiar / Tutor legal directo** | Persona con vínculo directo (padre, madre, cónyuge, hijo) que necesita seguimiento continuo del caso | Autenticado | Sí (registro con verificación) |
| A03 | **Voluntario verificador** | Persona de la sociedad civil capacitada (cruz roja, defensor civil, iglesias) que puede ayudar a verificar datos en campo | Autenticado + capacitación | Sí (con verificación de identidad) |
| A04 | **Agente policial verificador** | Miembro de la Policía Nacional autorizado para confirmar oficialmente el estado de un caso | Autenticado + credencial oficial | Sí (integración con sistema PN) |
| A05 | **Personal de salud / hospitalario** | Personal de emergencias, hospitales o clínicas que registra personas ingresadas no identificadas | Autenticado + credencial institucional | Sí (por institución) |
| A06 | **Administrador de plataforma** | Gestor operativo: supervisa ingestión de datos, monitorea fuentes, gestiona usuarios y resuelve incidencias | Autenticado (rol admin) | Sí (interno) |
| A07 | **Superadmin gubernamental** | Punto de contacto del Consejo Nacional de Alertas RD (Ley 25-26), puede emitir reportes consolidados y auditorías | Autenticado (rol superadmin) | Sí (designado por MIP/Consejo) |
| A08 | **Visitante anónimo** | Cualquier persona que consulta el buscador público o la landing institucional | No autenticado | No |

---

## 3. USER STORIES POR ACTOR

### 3.1 Ciudadano reportante (A01)

| ID | Historia | Prioridad | Criterios de aceptación (Given/When/Then) |
|---|---|---|---|
| US-01 | **Como** ciudadano reportante, **quiero** reportar una desaparición desde mi teléfono en menos de 3 minutos, **para** que la búsqueda comience lo antes posible sin tener que ir a una unidad policial de inmediato. | **P0** | **Given** un ciudadano con acceso a internet desde su móvil; **When** completa el formulario rápido de reporte (nombre, edad, sexo, última ubicación, foto opcional, su contacto); **Then** el sistema confirma en pantalla "Reporte recibido. Un agente lo revisará en breve" y envía un código de seguimiento por SMS/correo. |
| US-02 | **Como** ciudadano reportante, **quiero** adjuntar una foto de la persona desaparecida, **para** facilitar su identificación visual por otros ciudadanos y autoridades. | **P0** | **Given** un reporte en creación; **When** el usuario adjunta una foto; **Then** el sistema valida que sea imagen (JPG/PNG, <10MB), la redimensiona a 3 tamaños (thumbnail, web, original), extrae metadatos EXIF que puedan dar ubicación, y la asocia al reporte con marca de "pendiente de verificación". |
| US-03 | **Como** ciudadano reportante, **quiero** recibir una notificación automática si el sistema encuentra una posible coincidencia, **para** saber si mi familiar apareció o está identificado en un hospital. | **P1** | **Given** un reporte activo y un nuevo registro ingerido de fuente externa; **When** el motor de matching produce un match con score > umbral configurable; **Then** el sistema envía notificación (SMS + correo) al reportante con un mensaje como "Tenemos una posible coincidencia. Un verificador la está revisando. Te notificaremos en breve." |
| US-04 | **Como** ciudadano reportante, **quiero** marcar que ya encontré a la persona por mi cuenta, **para** cerrar el reporte y evitar que otros sigan buscando. | **P0** | **Given** un reporte activo; **When** el reportante confirma "Ya la/lo encontré" e ingresa un token de verificación (enviado a su teléfono); **Then** el reporte pasa a estado "Cerrado — Localizado" con marca de tiempo, y se envía notificación a los verificadores para confirmación. |
| US-05 | **Como** ciudadano reportante, **quiero** ver el estado actualizado de mi reporte en un panel simple, **para** no tener que llamar a instituciones preguntando. | **P1** | **Given** un código de seguimiento válido; **When** el usuario ingresa el código en la página de seguimiento; **Then** ve el estado actual (Recibido / En verificación / Coincidencia detectada / En investigación / Cerrado), la fecha de última actualización, y las acciones que puede tomar. |

### 3.2 Familiar / Tutor legal directo (A02)

| ID | Historia | Prioridad | Criterios de aceptación |
|---|---|---|---|
| US-06 | **Como** familiar directo, **quiero** registrarme como tutor legal del caso, **para** que el sistema me reconozca como contacto oficial y pueda autorizar o denegar la publicación de datos sensibles (fotos, ubicación exacta). | **P1** | **Given** un reporte existente; **When** el familiar se registra con su cédula y prueba de vínculo (o declara bajo fe); **Then** el sistema lo asocia al caso y le otorga permisos de gestión sobre los datos del reporte, previa validación de identidad por agente verificador. |
| US-07 | **Como** familiar directo, **quiero** restringir qué datos se muestran públicamente (ej: foto sí, ubicación exacta no), **para** proteger la privacidad de la persona buscada. | **P2** | **Given** un familiar autenticado en su panel; **When** modifica las preferencias de visibilidad; **Then** el sistema aplica los cambios en menos de 1 minuto en el frontend público. |

### 3.3 Voluntario verificador (A03)

| ID | Historia | Prioridad | Criterios de aceptación |
|---|---|---|---|
| US-08 | **Como** voluntario verificador, **quiero** recibir una cola de coincidencias pendientes de verificación (ordenadas por score y urgencia), **para** priorizar mi trabajo. | **P1** | **Given** matches generados por el motor; **When** el voluntario accede a su panel; **Then** ve una lista ordenada con: foto, nombres, score de coincidencia, tiempo desde detección, y botones "Confirmar coincidencia", "Descartar", "Necesito más datos". |
| US-09 | **Como** voluntario verificador, **quiero** marcar una coincidencia como "Confirmada — persona localizada" solo después de contactar al familiar y a la institución correspondiente, **para** evitar errores con consecuencias emocionales graves. | **P1** | **Given** una coincidencia en revisión; **When** el voluntario marca "Confirmada"; **Then** el sistema le exige una confirmación de dos pasos: (1) "¿Has contactado al familiar y confirmado la identidad?" (2) "¿Has verificado con la fuente (hospital/policía)?" Solo tras ambas confirmaciones se actualiza el estado. |
| US-10 | **Como** voluntario verificador, **quiero** ver el historial de cambios de un caso, **para** entender qué verificaciones previas se hicieron. | **P2** | **Given** un caso abierto; **When** el voluntario accede al detalle; **Then** ve línea de tiempo con: fecha, usuario que hizo el cambio, tipo de cambio, valor anterior y nuevo. |

### 3.4 Agente policial verificador (A04)

| ID | Historia | Prioridad | Criterios de aceptación |
|---|---|---|---|
| US-11 | **Como** agente policial, **quiero** recibir reportes ciudadanos nuevos en mi jurisdicción geográfica, **para** activar el protocolo Alertas RD dentro de las 24 horas que exige la ley. | **P0** | **Given** un reporte ciudadano geolocalizado en la provincia/municipio del agente; **When** el reporte es creado; **Then** el agente recibe una alerta en su panel con los datos del reporte y un botón "Asignar a unidad" que abre el flujo de activación de alerta. |
| US-12 | **Como** agente policial, **quiero** actualizar el estado de un caso a "Resuelto — Localizado con vida" o "Resuelto — Fallecido", **para** que el sistema refleje el desenlace oficial. | **P0** | **Given** un caso activo; **When** el agente selecciona el nuevo estado; **Then** el sistema solicita número de acta/informe policial, muestra advertencia ("Esta acción notificará al familiar. ¿Confirmar?"), registra auditoría, y notifica al familiar. |
| US-13 | **Como** agente policial, **quiero** exportar un reporte consolidado de casos activos en mi demarcación, **para** presentarlo en reunión del Consejo Nacional de Alertas RD. | **P2** | **Given** un agente autenticado; **When** solicita exportación; **Then** el sistema genera PDF con logo de la institución, tabla de casos activos, firmas digitales y código QR de verificación. |

### 3.5 Personal de salud / hospitalario (A05)

| ID | Historia | Prioridad | Criterios de aceptación |
|---|---|---|---|
| US-14 | **Como** personal de salud, **quiero** consultar rápidamente si un paciente ingresado no identificado coincide con algún reporte activo, **para** notificar al familiar en minutos, no en días. | **P1** | **Given** un paciente NN ingresado; **When** el personal ingresa datos básicos (sexo aproximado, rango etario, fecha de ingreso, características físicas, ubicación del hospital); **Then** el sistema devuelve una lista de posibles matches con score y datos públicos del reporte. |
| US-15 | **Como** personal de salud, **quiero** registrar que una persona no identificada ha sido ingresada en mi centro, **para** que el sistema automáticamente intente cruzarlo con reportes activos. | **P1** | **Given** personal autenticado de un centro de salud; **When** registra paciente NN (foto, características, fecha ingreso); **Then** el sistema crea un registro en Personas No Identificadas, ejecuta matching contra reportes activos, y notifica a verificadores si hay match >70%. |

### 3.6 Administrador de plataforma (A06)

| ID | Historia | Prioridad | Criterios de aceptación |
|---|---|---|---|
| US-16 | **Como** administrador, **quiero** ver el dashboard con métricas en tiempo real (casos activos, fuentes activas/caídas, matches pendientes, tiempo promedio de resolución), **para** tomar decisiones operativas. | **P1** | **Given** el admin en su dashboard; **Then** ve tarjetas con: casos activos hoy/esta semana/este mes, fuentes de datos con estado (✅ Activo / ❌ Caído / ⏸ Pausado), cola de matches pendientes, tiempo promedio de reporte a resolución. |
| US-17 | **Como** administrador, **quiero** supervisar las ejecuciones del scraping, **para** detectar si una fuente cambió su estructura HTML y necesita mantenimiento. | **P1** | **Given** la vista de ingestión; **When** un scraping falla más de 3 veces consecutivas; **Then** el sistema marca la fuente como "Caída — requiere atención" y envía alerta al admin con el log de error y el fragmento HTML problemático. |
| US-18 | **Como** administrador, **quiero** gestionar usuarios verificadores (aprobar, suspender, asignar zonas), **para** mantener la integridad del sistema. | **P1** | **Given** solicitudes de registro de voluntarios pendientes; **When** el admin las revisa; **Then** puede aprobar con asignación de provincia/municipio, rechazar con motivo, o suspender temporalmente. |

### 3.7 Superadmin gubernamental (A07)

| ID | Historia | Prioridad | Criterios de aceptación |
|---|---|---|---|
| US-19 | **Como** superadmin gubernamental, **quiero** generar reportes de auditoría completos (quién creó/modificó/eliminó cada registro), **para** rendir cuentas al Consejo Nacional de Alertas RD. | **P2** | **Given** una solicitud de auditoría; **When** se especifica rango de fechas y tipo de operación; **Then** el sistema exporta CSV/PDF inalterable con firma de integridad (hash SHA-256 por registro). |
| US-20 | **Como** superadmin gubernamental, **quiero** configurar los umbrales del motor de matching (score mínimo, ponderación por campos), **para** ajustar la sensibilidad según la etapa del proyecto. | **P2** | **Given** acceso a configuración del motor; **When** se modifican parámetros; **Then** el sistema registra el cambio en auditoría y aplica el nuevo umbral en la próxima ejecución del matching. |

### 3.8 Visitante anónimo (A08)

| ID | Historia | Prioridad | Criterios de aceptación |
|---|---|---|---|
| US-21 | **Como** visitante anónimo, **quiero** buscar personas desaparecidas por nombre, edad aproximada, provincia o fecha, **para** saber si alguien que conozco o he visto está reportado. | **P0** | **Given** la página de búsqueda pública; **When** ingreso uno o más filtros; **Then** el sistema muestra resultados paginados con: foto (si autorizada), nombre, edad, provincia, fecha de desaparición, estado (Desaparecido / Localizado), y enlace a detalle público. |
| US-22 | **Como** visitante anónimo, **quiero** ver información general del proyecto en una landing clara, **para** entender qué hace, quién lo respalda, y cómo contactar a las autoridades oficiales. | **P0** | **Given** la landing page; **Then** el usuario ve: propósito del proyecto, qué NO es (no sustituye al 911/Policía), botones de acción principales ("Reportar desaparición", "Buscar persona"), enlaces a Policía Nacional, 911 y Alertas RD, y una sección de transparencia sobre datos de prueba vs. reales. |

---

## 4. CASOS DE USO CRÍTICOS PRIORIZADOS

### P0 — Bloqueante para MVP (debe funcionar en la demo)

| ID | Caso de uso | Actor primario | Módulo |
|---|---|---|---|
| CU-01 | Reportar persona desaparecida desde web móvil | Ciudadano reportante | Módulo 1 |
| CU-02 | Buscar personas desaparecidas con filtros | Visitante anónimo | Módulo 6 |
| CU-03 | Marcar persona como localizada (cierre de caso) | Familiar / Agente policial | Módulo 2 |
| CU-04 | Ingesta automática desde 1 fuente externa (Policía Nacional — boletines) | Sistema (background job) | Módulo 3 |
| CU-05 | Matching difuso entre reportes ciudadanos y registros ingeridos | Sistema (background job) | Módulo 4 |
| CU-06 | Verificar / descartar una coincidencia del motor | Verificador | Módulo 5 |
| CU-07 | Landing institucional con propósito y limitaciones | Visitante anónimo | Módulo 12 |
| CU-08 | Exportar reporte individual en PDF imprimible | Cualquier autenticado | Módulo 11 |

### P1 — Importante (MVP+ / primera iteración post-MVP)

| ID | Caso de uso | Actor primario | Módulo |
|---|---|---|---|
| CU-09 | Recibir notificación de posible coincidencia | Ciudadano reportante | Módulo 9 |
| CU-10 | Panel de seguimiento de reporte (código) | Ciudadano reportante | Módulo 1 |
| CU-11 | Dashboard administrativo con métricas | Administrador | Módulo 8 |
| CU-12 | Registro de paciente NN en hospital | Personal de salud | Módulo 2 |
| CU-13 | Consulta rápida de coincidencias desde hospital | Personal de salud | Módulo 5 |
| CU-14 | Gestión de usuarios verificadores | Administrador | Módulo 5 |
| CU-15 | Ingesta desde 2+ fuentes (datos.gob.do + hospital público) | Sistema (background job) | Módulo 3 |

### P2 — Futuro (plataforma completa)

| ID | Caso de uso | Actor primario | Módulo |
|---|---|---|---|
| CU-16 | Mapa de casos activos por provincia/municipio | Visitante anónimo | Módulo 7 |
| CU-17 | Reportes de auditoría completos | Superadmin gubernamental | Módulo 10 |
| CU-18 | Configuración de umbrales de matching | Superadmin gubernamental | Módulo 4 |
| CU-19 | App móvil nativa (iOS/Android) | Todos los actores | Futuro |
| CU-20 | Integración API directa con Registro Nacional (Alertas RD) | Sistema / Policía | Futuro |
| CU-21 | IA para priorización de casos por nivel de riesgo | Sistema | Futuro |
| CU-22 | Notificaciones push a voluntarios cercanos (geocercas) | Voluntario | Futuro |

---

## 5. RESTRICCIONES LEGALES Y ÉTICAS

### 5.1 Ley 172-13 de Protección de Datos Personales (RD)

A continuación se identifican los artículos específicos de la Ley 172-13 que impactan el diseño del sistema:

| Artículo | Contenido | Implicación para el sistema |
|---|---|---|
| **Art. 1** | Objeto: protección integral de datos personales en archivos públicos/privados. | Toda la plataforma debe diseñarse con protección de datos como principio fundacional (privacy by design). |
| **Art. 2** | Alcance: aplica a datos personales en cualquier banco de datos, público o privado. | La plataforma cae bajo el alcance de la ley. Debe cumplir con todos los principios. |
| **Art. 3 (principios)** | **Licitud**: fines legítimos y no contrarios al orden público. | El fin es lícito (búsqueda de personas desaparecidas, interés público). Debe declararse explícitamente. |
| **Art. 3 (principios)** | **Consentimiento informado**: previo, libre, inequívoco y específico del titular. | Al reportar a un tercero (persona desaparecida), no se puede obtener su consentimiento. Fundamento legal: interés público vital + protección de la salud/vida (art. 5, excepción). Documentar en DPO. |
| **Art. 3 (principios)** | **Calidad**: datos exactos, actualizados, veraces. | Los datos ingeridos por scraping llevan marca de "no verificados" hasta confirmación. El sistema debe permitir corrección rápida. |
| **Art. 3 (principios)** | **Finalidad**: datos usados solo para el propósito que motivó su recolección. | Prohibido usar datos de la plataforma para otros fines (marketing, estudios no autorizados). Términos de uso deben ser explícitos. |
| **Art. 3 (principios)** | **Seguridad**: medidas técnicas y organizativas adecuadas. | Cifrado en reposo (AES-256) y tránsito (TLS 1.3), control de acceso basado en roles, logging de auditoría. |
| **Art. 3 (principios)** | **Deber de secreto**: quienes intervienen en el tratamiento deben guardar confidencialidad. | Todos los usuarios con acceso a datos sensibles (verificadores, admin, personal salud) deben firmar acuerdo de confidencialidad. |
| **Art. 4** | **Datos sensibles** (origen racial, salud, orientación sexual, creencias): protección reforzada. | El sistema potencialmente trata datos de salud (condición médica de la persona, lesiones). Requiere consentimiento expreso y escrito del titular (o, en su defecto, del familiar tutor). Almacenar con cifrado adicional y acceso restringido. |
| **Art. 10** | Derecho de acceso: toda persona puede acceder a los datos que sobre ella reposen. | Si la persona desaparecida aparece, tiene derecho a solicitar la eliminación de sus datos de la plataforma. Implementar flujo de "solicitud de eliminación de datos personales". |
| **Art. 17** | Acción de hábeas data. | El sistema debe poder responder a requerimientos judiciales de exhibición, rectificación o eliminación de datos en plazos legales. |

### 5.2 Protección de menores de edad (Ley 172-13 + Ley 136-03 + Ley 25-26)

| Principio | Implicación técnica |
|---|---|
| **Interés superior del menor** (Ley 136-03, Art. 5) | En caso de duda sobre si publicar o no datos de un menor, la decisión debe inclinarse siempre por no publicar. |
| **Alerta Amber** (Ley 25-26, Art. 25) | Los casos de menores desaparecidos deben tener un flujo especial con activación prioritaria. Los datos públicos de menores deben ser mínimos (solo inicial del nombre, edad aproximada, provincia). |
| **Foto de menores** | No publicar foto de frente de un menor sin autorización expresa del tutor. En su lugar, usar silueta genérica + descripción textual. En la demo/MVP, usar exclusivamente datos sintéticos de menores. |
| **Consentimiento para tratamiento de datos de menores** | Requiere autorización del tutor legal. Si el reportante no es el tutor, el sistema debe activar un flujo de verificación de tutela antes de publicar datos del menor. |
| **Datos de salud de menores** | Nunca publicar condiciones médicas preexistentes. Solo almacenar para uso interno de autoridades y personal de salud autorizado. |

### 5.3 Ley 25-26 "Alertas RD" — Implicaciones funcionales

| Mandato legal | Implicación para el sistema |
|---|---|
| **Registro Nacional de Personas Desaparecidas** (Art. 5) | La plataforma debe diseñarse para interoperar con este registro, no para reemplazarlo. El MVP puede operar en paralelo con datos sintéticos; la integración real requiere convenio con la Policía Nacional. |
| **Activación en <24h** (Art. 12) | El flujo de reporte ciudadano debe priorizar que el reporte llegue a la autoridad competente (Policía/MP) en menos de 24h. El sistema debe tener un mecanismo de escalamiento automático si el caso no ha sido asignado en 12h. |
| **Alertas tipificadas**: Amber, Silver, Azul, Rosa (Arts. 25-28) | El formulario de reporte debe incluir la categorización de alerta según perfil de la persona. La interfaz debe adaptar el tono y las acciones según el tipo de alerta (ej: Rosa -> ofrecer enlace a Ministerio de la Mujer). |
| **Consejo Nacional** (Art. 6) | El superadmin del sistema debe tener capacidad de generar reportes en el formato que requiera el Consejo. |
| **Difusión masiva** (Art. 18) | El sistema puede servir como un canal de difusión complementario, mostrando alertas activas en la landing page y ofreciendo compartir en redes. |

### 5.4 Principios éticos no negociables

1. **No suplantación de autoridad**: en cada punto de contacto se deja claro: "Esta plataforma es complementaria. Si estás en una emergencia, llama al 911 o acude a la unidad policial más cercana."
2. **Honestidad demo vs. producción**: los datos sintéticos y las integraciones simuladas deben identificarse claramente con etiquetas: "🟡 Dato de prueba — no corresponde a un caso real" en la demo.
3. **Cero tolerancia a la exposición de datos de menores**: implementar protecciones reforzadas. En el MVP, los casos de menores deben ser manejados exclusivamente por verificadores con capacitación específica.
4. **Derecho al olvido**: cualquier persona localizada (o su familiar) puede solicitar la eliminación completa de sus datos de la plataforma una vez resuelto el caso.
5. **Transparencia algorítmica**: el score de coincidencia del motor de matching debe ser explicable ("Este match se basa en coincidencia de nombre (85%) + edad (90%) + ubicación (70%)").
6. **No comerciar con el dolor**: prohibición absoluta de publicidad, venta de datos, o cualquier forma de monetización de los casos. La plataforma es un bien público.

---

## 6. GLOSARIO

| Término | Definición |
|---|---|
| **Reporte** | Registro creado por un ciudadano indicando que una persona está desaparecida. |
| **Match / Coincidencia** | Resultado del motor de matching que sugiere que dos registros (reporte + fuente externa) podrían referirse a la misma persona. |
| **Ingesta** | Proceso automatizado de obtención de datos desde fuentes externas (scraping). |
| **Fuente** | Origen de datos externo (sitio web, API, PDF). |
| **Registro Nacional** | Base de datos centralizada creada por Ley 25-26, administrada por la Policía Nacional. |
| **NN / No identificado** | Paciente ingresado en un centro de salud sin identidad conocida. |
| **Estado del caso** | Valor en {Recibido, En verificación, Coincidencia detectada, En investigación, Cerrado — Localizado, Cerrado — Fallecido, Cerrado — Sin resolver}. |
| **Score de matching** | Valor 0-100% que indica la confianza de que dos registros corresponden a la misma persona. |
| **Alerta RD** | Mecanismo de difusión masiva creado por Ley 25-26, con subtipos Amber, Silver, Azul, Rosa. |

---

*Fin de la FASE 1. Pendiente de validación del usuario antes de continuar a FASE 2 (especificación técnica).*
