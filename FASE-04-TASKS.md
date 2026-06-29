# FASE 4 — PLAN DE TAREAS (tasks.md)

> **Proyecto:** Plataforma ciudadana de cruce de información sobre personas desaparecidas — República Dominicana
> **Versión:** 1.0-draft
> **Propósito:** Desglose del MVP en tareas técnicas, Definition of Done, dependencias, y roadmap de evolución.

---

## 1. MVP — DESGLOSE POR MÓDULOS

### Convenciones

- **ID**: `T-XXX` (tarea), `M-XX` (módulo)
- **DoD** = Definition of Done (criterios que deben cumplirse para considerar la tarea completa)
- **Skill**: referencia al skill de FASE-0 requerido
- **Estimación**: en horas ideales de desarrollo
- **Dependencia**: tareas que deben estar completas antes de empezar

---

## MÓDULO 0 — PROYECTO BASE Y ARQUITECTURA

| ID | Tarea | DoD | Dependencias | Skill | Est. |
|---|---|---|---|---|---|
| T-001 | Crear solución .NET 8 con Clean Architecture: 4 proyectos (Domain, Application, Infrastructure, Web) | Solución compila, proyectos referenciados correctamente, carpetas creadas con estructura estándar | — | S-06 | 2h |
| T-002 | Configurar DbContext, cadena conexión SQL Server, y primera migración | `dotnet ef migrations add InitialCreate` ejecuta sin errores, BD creada en SQL Server | T-001 | S-06 | 2h |
| T-003 | Implementar Repository<T> genérico + UnitOfWork + interfaces en Domain | Tests unitarios de repositorio pasan, CRUD básico funcional | T-002 | S-06 | 4h |
| T-004 | Configurar Serilog (archivo + consola + SQL Server), middleware de error handling global | Logs se escriben en los 3 sinks, error handling devuelve JSON consistente | T-001 | S-06 | 2h |
| T-005 | Configurar Tailwind CSS (compilado con CLI o CDN), layout base _Layout.cshtml con header/footer/nav | Layout responsive se renderiza en todas las páginas, Tailwind utilitarias funcionan | T-001 | S-10 | 2h |
| T-006 | Configurar AutoMapper + FluentValidation global | Mappings se registran automáticamente, validación se dispara en ModelState | T-001 | S-06 | 1h |
| T-007 | Configurar Quartz.NET con scheduler persistente | Job de prueba se ejecuta en el intervalo configurado, log se escribe | T-002 | S-12 | 2h |
| T-008 | Configurar proyecto de tests (xUnit + Moq + FluentAssertions) | `dotnet test` corre y pasa 1 test de ejemplo | T-001 | S-06 | 1h |

**Total M0: ~16h**

---

## MÓDULO 1 — REPORTE CIUDADANO DE PERSONA DESAPARECIDA

| ID | Tarea | DoD | Dependencias | Skill | Est. |
|---|---|---|---|---|---|
| T-009 | Crear entidades del core: PersonaReportada, Reporte, EstadoCaso, ZonaGeografica | Entidades creadas en Domain, migración generada, BD actualizada | T-002 | S-06 | 3h |
| T-010 | Crear seed data de Estados de Caso y ZonasGeográficas (32 provincias + DN + municipios) | Seed ejecutado, datos disponibles en BD | T-009 | S-06 | 2h |
| T-011 | Implementar CreateReportUseCase con validación (FluentValidation) | Tests pasan: reporte válido → OK, campos inválidos → error específico | T-009, T-006 | S-06 | 4h |
| T-012 | Crear ViewModel y Razor View del formulario step-by-step (6 pasos) | Formulario se renderiza, navegación entre pasos funciona, datos persisten en sesión | T-005, T-011 | S-03, S-10 | 6h |
| T-013 | Implementar subida de foto con redimensionamiento (3 tamaños) + watermark | Foto se sube, se redimensiona, watermark con ID se aplica, se guarda en FileStorage | T-009 | S-06 | 3h |
| T-014 | Implementar flujo de verificación de contacto (enviar código SMS/email) | Código se envía, formulario espera confirmación, reporte queda "Pendiente" hasta verificación | T-011 | S-07 | 3h |
| T-015 | Implementar pantalla de confirmación con código de seguimiento | Código se genera (formato LP-XXXXX), se muestra, se envía al contacto | T-014 | S-03 | 2h |
| T-016 | Implementar GetReportStatusUseCase (por código de seguimiento) | Endpoint devuelve estado actual del reporte con línea de tiempo | T-009 | S-06 | 2h |
| T-017 | Crear vista de seguimiento por código | Vista se renderiza con datos del reporte, línea de tiempo, acciones disponibles | T-016, T-005 | S-03 | 2h |

**Total M1: ~27h**

---

## MÓDULO 2 — REPORTE DE PERSONA LOCALIZADA

| ID | Tarea | DoD | Dependencias | Skill | Est. |
|---|---|---|---|---|---|
| T-018 | Implementar CloseReportUseCase (marcar como localizada) con verificación de token | Test: cierre sin token → error. Cierre con token válido → estado cambia a "Localizado" | T-009 | S-03, S-07 | 3h |
| T-019 | Crear formulario de "Reportar persona localizada" (búsqueda por código + nombre) | Formulario se renderiza, busca persona, muestra datos para confirmar | T-018, T-005 | S-10 | 3h |
| T-020 | Implementar flujo de verificación para cierre: contacto al familiar + confirmación | Doble confirmación: (1) notificar al familiar, (2) esperar confirmación del verificador | T-018, T-014 | S-07 | 3h |
| T-021 | Crear vista de "Persona localizada" con mensaje de cierre | Vista muestra estado "✅ Localizada", fecha, y mensaje de agradecimiento | T-019, T-005 | S-03 | 1h |

**Total M2: ~10h**

---

## MÓDULO 3 — INGESTA DE DATOS EXTERNOS

| ID | Tarea | DoD | Dependencias | Skill | Est. |
|---|---|---|---|---|---|
| T-022 | Crear entidades: FuenteDatos, RegistroIngerido | Entidades en Domain, migración, BD actualizada | T-002 | S-06 | 2h |
| T-023 | Implementar interface IDataSourceConnector + IngestionResult | Interfaz definida en Domain, implementación base creada | T-022 | S-04 | 2h |
| T-024 | Implementar DataSourceConnectorFactory (selecciona conector por tipo de fuente) | Factory retorna el conector correcto dado un FuenteDatos.Type | T-023 | S-04 | 1h |
| T-025 | Implementar HttpClient resiliente con Polly (retry, circuit breaker, timeout) | Test: HTTP 503 → retry 3 veces. HTTP 429 → espera Retry-After. Timeout 30s. | T-023 | S-04, S-12 | 3h |
| T-026 | Implementar PoliciaNacionalConnector (scraper HTML con AngleSharp) | Conector extrae registros de la URL objetivo, parsea con selectores CSS, devuelve IngestionResult | T-025 | S-04 | 6h |
| T-027 | Implementar DatosGobDoConnector (cliente API REST CKAN) | Conector consume API de datos.gob.do, parsea JSON, devuelve IngestionResult | T-025 | S-04, S-08 | 4h |
| T-028 | Implementar HospitalSimuladoConnector (genera datos sintéticos para demo) | Conector genera registros sintéticos verosímiles con Bogus, flag DatosSinteticos=true | T-023 | S-04 | 2h |
| T-029 | Implementar pipeline de ingestión: Fetch → Parse → Extract → Validate → Dedup → Store → Log | Pipeline completo ejecuta todos los pasos, logs en cada etapa, maneja errores parciales | T-024, T-026, T-027 | S-04 | 6h |
| T-030 | Implementar ScrapingSchedulerJob (Quartz.NET) con schedule configurable por fuente | Job se ejecuta según el IntervaloMinutos de cada FuenteDatos, resultados se almacenan | T-029, T-007 | S-12 | 3h |
| T-031 | Implementar detección de cambio estructural (0 registros detectados → alerta admin) | Si 2 ejecuciones consecutivas devuelven 0 registros donde antes >0, fuente marcada "Caída" | T-029 | S-04 | 2h |
| T-032 | Crear seed de fuentes de datos (PN, datos.gob.do, HospitalSimulado) | Seed ejecutado, fuentes configuradas en BD, schedules activos | T-022, T-030 | S-04, S-08 | 1h |

**Total M3: ~32h**

---

## MÓDULO 4 — MOTOR DE COINCIDENCIAS (MATCHING)

| ID | Tarea | DoD | Dependencias | Skill | Est. |
|---|---|---|---|---|---|
| T-033 | Implementar FuzzySharp wrapper con algoritmos: JaroWinkler, Levenshtein, TokenSortRatio | Tests: "Juan Pérez" ≈ "Juan C. Pérez" → score >0.8. "Pedro" vs "Juan" → score <0.3 | — | S-05 | 4h |
| T-034 | Implementar NormalizadorNombresHispanos (quitar acentos, expandir abreviaturas, manejar "De", "Del", "De la") | Test: "José María" → "Jose Maria", "Mª del Carmen" → "Maria del Carmen" | T-033 | S-05 | 3h |
| T-035 | Implementar BlockingKeyGenerator (agrupa por primeras letras del apellido) | Test: "Pérez" → "PE", "Martínez" → "MA" | T-033 | S-05 | 1h |
| T-036 | Implementar ScoreWeightCalculator con ponderación configurable | Test: score general se calcula correctamente según pesos. Umbral configurable desde DB. | T-033 | S-05 | 3h |
| T-037 | Implementar MatchingJob (Quartz.NET): procesa RegistrosIngeridos no procesados contra Reportes activos | Job se ejecuta, crea Coincidencia para cada par que supera umbral, marca registros como procesados | T-035, T-036, T-007 | S-05, S-12 | 6h |
| T-038 | Implementar almacenamiento de resultados de matching (entidad Coincidencia) | Coincidencias se persisten en BD con scores desglosados | T-022, T-033 | S-05 | 2h |
| T-039 | Implementar re-matching cuando se crea un nuevo reporte (matching incremental) | Al crear un reporte, se ejecuta matching contra todos los registros ingeridos no vinculados | T-037 | S-05 | 3h |

**Total M4: ~22h**

---

## MÓDULO 5 — PANEL DE VERIFICACIÓN

| ID | Tarea | DoD | Dependencias | Skill | Est. |
|---|---|---|---|---|---|
| T-040 | Crear vista de cola de coincidencias pendientes (ordenada por score descendente) | Vista renderiza lista con foto, nombres, score, badge de prioridad | T-005, T-038 | S-10 | 4h |
| T-041 | Implementar vista de comparación lado a lado (reporte vs. registro ingerido) | Vista muestra ambos conjuntos de datos en paralelo con coincidencias resaltadas | T-040 | S-10 | 3h |
| T-042 | Implementar acción "Confirmar coincidencia" con doble verificación (contactar familiar + institución) | Botón requiere 2 confirmaciones antes de ejecutar cambio de estado | T-041 | S-07 | 3h |
| T-043 | Implementar acción "Descartar coincidencia" con motivo obligatorio | Descartar requiere seleccionar motivo (falsos datos, persona diferente, error del sistema) | T-041 | S-06 | 2h |
| T-044 | Implementar acción "Solicitar más datos" (envía notificación al reportante) | Se envía notificación al reportante pidiendo datos adicionales específicos | T-041, T-055 | S-06 | 2h |

**Total M5: ~14h**

---

## MÓDULO 6 — BUSCADOR PÚBLICO

| ID | Tarea | DoD | Dependencias | Skill | Est. |
|---|---|---|---|---|---|
| T-045 | Implementar SearchPersonsUseCase con filtros (nombre, edad, provincia, estado, fecha, sexo, tipo alerta) | Tests: búsqueda sin filtros → todos los activos. Con filtro nombre → LIKE. Combinación de filtros → AND. | T-009 | S-06 | 4h |
| T-046 | Implementar paginación con offset/limit | Respuesta incluye total, page, pageSize, totalPages. Navegación entre páginas funciona. | T-045 | S-06 | 1h |
| T-047 | Crear vista de buscador con formulario de filtros y resultados en cards | Vista responsive: 1 col móvil, 2 cols tablet, 3 cols desktop. Filtros colapsables en móvil. | T-045, T-005 | S-10 | 4h |
| T-048 | Crear vista de detalle de persona (pública) con línea de tiempo | Vista muestra: foto, datos, línea de tiempo, botón "Tengo información", enlaces a autoridades | T-009, T-005 | S-03, S-10 | 3h |
| T-049 | Implementar estado "vacío" y "error" en buscador | Sin resultados → mensaje + sugerencia. Error → mensaje + reintentar. | T-047 | S-10 | 1h |

**Total M6: ~13h**

---

## MÓDULO 7 — MAPA DE CASOS ACTIVOS (P2, no incluido en MVP)

> Postergado a fase post-MVP. No se detalla en este plan.

---

## MÓDULO 8 — DASHBOARD ADMINISTRATIVO

| ID | Tarea | DoD | Dependencias | Skill | Est. |
|---|---|---|---|---|---|
| T-050 | Implementar GetDashboardMetricsUseCase (casos activos, resueltos, tiempo promedio, fuentes activas/caídas, matches pendientes) | Tests: métricas correctas con data sintética. Tiempo promedio calculado correctamente. | T-009, T-022 | S-06 | 4h |
| T-051 | Crear vista de dashboard admin con tarjetas de métricas, gráficos de tendencia (Chart.js simple) | Vista renderiza: 4 tarjetas top, gráfico de tendencia semanal, tabla de fuentes con estado | T-050, T-005 | S-10 | 4h |
| T-052 | Implementar vista de estado de fuentes de datos (verde/rojo/amarillo, última ejecución, total registros) | Vista muestra tabla de fuentes con indicador visual de salud | T-050, T-005 | S-10 | 2h |

**Total M8: ~10h**

---

## MÓDULO 9 — NOTIFICACIONES

| ID | Tarea | DoD | Dependencias | Skill | Est. |
|---|---|---|---|---|---|
| T-053 | Crear entidad Notificacion + NotificationRepository | Entidad en Domain, migración, CRUD básico | T-002 | S-06 | 1h |
| T-054 | Implementar INotificationService (SmsService + EmailService) | Interfaces definidas, implementaciones con Twilio + MailKit/SendGrid | T-053 | S-06 | 3h |
| T-055 | Implementar NotificationDispatchJob (Quartz.NET): procesa notificaciones pendientes y las envía | Job recoge notificaciones no enviadas, las envía por el canal configurado, marca como enviadas | T-054, T-007 | S-12 | 3h |
| T-056 | Implementar generación automática de notificaciones: match encontrado, caso resuelto, solicitud de datos | Tests: al crear match > umbral → notificación creada. Al cerrar caso → notificación creada. | T-054, T-038, T-018 | S-06 | 3h |
| T-057 | Crear vista de bandeja de notificaciones en panel de usuario | Vista muestra lista de notificaciones, leídas/no leídas, link a caso relacionado | T-005, T-053 | S-10 | 2h |

**Total M9: ~12h**

---

## MÓDULO 10 — AUDITORÍA Y TRAZABILIDAD

| ID | Tarea | DoD | Dependencias | Skill | Est. |
|---|---|---|---|---|---|
| T-058 | Crear entidad Auditoria + AuditRepository | Entidad en Domain, migración, configuración de tamaño de columnas ValorAnterior/ValorNuevo | T-002 | S-06 | 1h |
| T-059 | Implementar AuditInterceptor (EF Core SaveChanges interceptor) que registra automáticamente cambios en entidades auditables | Test: al crear/actualizar/eliminar entidad auditada, se crea registro de auditoría automáticamente | T-058 | S-06 | 4h |
| T-060 | Implementar auditoría de acceso a datos sensibles (lectura de condiciones médicas, datos de menores) | Log de auditoría para SELECTs sobre columnas sensibles | T-059 | S-01, S-09 | 2h |
| T-061 | Crear vista de auditoría para SuperAdmin con filtros (fecha, usuario, entidad, operación) | Vista renderiza tabla paginada con filtros | T-005, T-058 | S-10 | 3h |

**Total M10: ~10h**

---

## MÓDULO 11 — EXPORTACIÓN DE REPORTES

| ID | Tarea | DoD | Dependencias | Skill | Est. |
|---|---|---|---|---|---|
| T-062 | Implementar PdfExportService (QuestPDF) para reporte individual de persona | PDF generado incluye: logo, datos de la persona, foto, línea de tiempo, código QR de verificación | T-009 | S-06 | 3h |
| T-063 | Implementar ExcelExportService (ClosedXML) para listado de casos (admin) | Excel generado con columnas: nombre, edad, provincia, estado, fecha reporte, fecha resolución | T-050 | S-06 | 2h |
| T-064 | Crear botón de "Exportar PDF" en detalle de persona (solo autenticados) | Botón visible en vista detalle, descarga PDF | T-062, T-005 | S-10 | 1h |

**Total M11: ~6h**

---

## MÓDULO 12 — LANDING INSTITUCIONAL

| ID | Tarea | DoD | Dependencias | Skill | Est. |
|---|---|---|---|---|---|
| T-065 | Crear vista de landing page con: hero, sección de alertas activas, cómo funciona, estadísticas, transparencia, footer con enlaces institucionales | Vista completa renderizada, responsive, todos los textos en español dominicano | T-005 | S-03, S-10 | 6h |
| T-066 | Implementar sección de alertas activas en landing (top 3-5 alertas) | Sección muestra alertas activas con badge de color según tipo, link a detalle | T-065, T-009 | S-10 | 2h |
| T-067 | Implementar sección de transparencia con disclaimer legal | Texto visible: "Esta plataforma es COMPLEMENTARIA. No sustituye a la Policía Nacional ni al 911." | T-065 | S-02, S-03 | 1h |
| T-068 | Crear página /sobre con descripción del proyecto, equipo, financiamiento | Vista renderizada con contenido institucional | T-005 | S-10 | 2h |
| T-069 | Crear página /contacto con enlaces a autoridades (Policía, 911, Alertas RD, Asodofade) | Vista renderizada con tarjetas de cada institución, teléfono, sitio web | T-005 | S-10 | 1h |

**Total M12: ~12h**

---

## MÓDULO 13 — SEGURIDAD Y USUARIOS

| ID | Tarea | DoD | Dependencias | Skill | Est. |
|---|---|---|---|---|---|
| T-070 | Implementar entidades Usuario, Rol, SesionUsuario + migración | Entidades en Domain, BD actualizada | T-002 | S-06 | 2h |
| T-071 | Implementar registro de usuario (email + password + verificación SMS/correo) | Test: registro → código enviado → confirmación → usuario activo | T-070 | S-07 | 4h |
| T-072 | Implementar login con JWT (access + refresh tokens) | Login retorna tokens, refresh token rotación, logout invalida sesión | T-071 | S-06, S-07 | 4h |
| T-073 | Implementar Role-based authorization middleware | Tests: endpoint de admin → 401 sin rol, endpoint de verificador → OK con rol | T-072 | S-06 | 2h |
| T-074 | Implementar rate limiting middleware (por IP + por usuario) | Tests: más de N requests/min → 429 Too Many Requests | T-001 | S-07 | 2h |
| T-075 | Implementar bloqueo de cuenta por intentos fallidos (5 intentos → 30 min) | Tests: 5 login fallidos → cuenta bloqueada, login correcto después de 30 min | T-072 | S-07 | 2h |
| T-076 | Implementar seed de usuarios de prueba (admin, verificador, familiar, personal salud) | Seed ejecutado, usuarios creados con roles correctos | T-070 | S-06 | 1h |

**Total M13: ~17h**

---

## MÓDULO 14 — DATOS DE PRUEBA (SEED)

| ID | Tarea | DoD | Dependencias | Skill | Est. |
|---|---|---|---|---|---|
| T-077 | Generar seed de personas desaparecidas sintéticas (50 casos) con Bogus | Personas creadas con DatosSinteticos=true, variedad de edades, sexos, provincias | T-009 | S-06 | 2h |
| T-078 | Generar seed de registros ingeridos sintéticos (40 registros, algunos coincidentes) | Registros creados con matches parciales contra las personas sintéticas | T-077, T-022 | S-05 | 2h |
| T-079 | Generar seed de coincidencias sintéticas (20 matches con varios scores) | Coincidencias creadas, algunas para revisar, algunas revisadas | T-078, T-038 | S-05 | 1h |

**Total M14: ~5h**

---

## 2. DEPENDENCIAS Y ORDEN DE IMPLEMENTACIÓN

### 2.1 Ruta crítica del MVP

```
T-001 (solución base)
  └─ T-002 (DbContext + BD)
       ├─ T-009 (entidades core) → T-011 (report use case) → T-012 (vista reporte) → ...
       ├─ T-022 (entidades fuente) → T-023 (IDataSourceConnector) → T-029 (pipeline) → ...
       ├─ T-033 (fuzzy) → T-037 (matching job) → T-038 (coincidencias)
       └─ T-070 (usuarios) → T-072 (JWT auth) → T-073 (roles)
```

### 2.2 Secuencia recomendada (iteraciones)

| Iteración | Tareas | Duración est. | Entregable |
|---|---|---|---|
| **I-0 — Base** | T-001 a T-008 | ~16h | Proyecto compila, BD lista, layout base |
| **I-1 — Landing + Buscador** | T-065 a T-069, T-045 a T-049 | ~25h | Landing pública + buscador funcional |
| **I-2 — Reporte ciudadano** | T-009 a T-017 | ~27h | Reporte funcional + fotos + SMS |
| **I-3 — Usuarios + Auth** | T-070 a T-076 | ~17h | Login, registro, roles, seguridad |
| **I-4 — Ingestión scraping** | T-022 a T-032 | ~32h | Scraping desde 2 fuentes reales + demo |
| **I-5 — Matching engine** | T-033 a T-039 | ~22h | Coincidencias automáticas generándose |
| **I-6 — Verificación** | T-040 a T-044 | ~14h | Verificador revisa y confirma/descarta |
| **I-7 — Cierre de caso** | T-018 a T-021 | ~10h | Persona localizada, caso cerrado |
| **I-8 — Dashboard + Admin** | T-050 a T-052 | ~10h | Métricas, monitoreo de fuentes |
| **I-9 — Notificaciones** | T-053 a T-057 | ~12h | Alertas SMS/correo automáticas |
| **I-10 — Auditoría + Export** | T-058 a T-064 | ~16h | Trazabilidad, PDF, Excel |
| **I-11 — Seed data demo** | T-077 a T-079 | ~5h | Demo presentable con datos sintéticos |

**Total estimado MVP: ~206 horas ideales de desarrollo**

---

## 3. ROADMAP MVP → PLATAFORMA NACIONAL COMPLETA

### 3.1 Etapas

```
FASE ACTUAL ──────────────────────────────────────────────────────────
│
├── 🏁 MVP (ahora — 3 meses)
│   ├── Funcionalidad core: reportar, buscar, verificar, cerrar
│   ├── Scraping: 2-3 fuentes (Policía Nacional + datos.gob.do + demo hospital)
│   ├── Matching difuso funcional
│   ├── Panel de verificación
│   ├── Dashboard admin básico
│   ├── Notificaciones SMS/email
│   ├── Landing + transparencia
│   └── Datos sintéticos para demo
│
├── 🚀 FASE 2 — Integración institucional (3-6 meses post-MVP)
│   ├── Convenio con Policía Nacional → API directa al Registro Nacional
│   ├── Convenio con SNS → feed hospitalario de pacientes NN
│   ├── Integración con 9-1-1 (reportes de personas extraviadas)
│   ├── Convenio con Procuraduría/INACIF → datos forenses
│   ├── Autenticación 2FA para verificadores y agentes
│   ├── Mapa de casos activos por provincia/municipio (Leaflet)
│   └── API pública oficial para consumo de terceros
│
├── 📱 FASE 3 — Mobile + Alcance nacional (6-12 meses post-MVP)
│   ├── App móvil nativa (Flutter / .NET MAUI)
│   ├── Notificaciones push
│   ├── Geocercas: alertar a voluntarios cercanos cuando hay un nuevo caso
│   ├── Reporte por voz / asistente (para usuarios con baja alfabetización digital)
│   ├── Integración con redes sociales (compartir alertas)
│   ├── Módulo de voluntarios: registro, capacitación, asignación geográfica
│   └── Portal de transparencia pública con datos abiertos descargables
│
├── 🤖 FASE 4 — IA y Analítica Avanzada (12-18 meses)
│   ├── IA para priorización de casos (predicción de riesgo por perfil)
│   ├── Reconocimiento facial (solo con autorización judicial y marco legal)
│   ├── Análisis de patrones de desaparición por zona/temporada
│   ├── Chatbot informativo 24/7 para familiares
│   ├── Dashboard ejecutivo para el Consejo Nacional de Alertas RD
│   └── Reportes automáticos al Congreso / Ministerio Público
│
└── 🌐 FASE 5 — Sostenibilidad y Escala (18-24 meses)
    ├── Interoperabilidad con INTERPOL / sistemas regionales
    ├── Convenios con aeropuertos, terminales, fronteras (integración automatizada)
    ├── Modelo de replicabilidad para otros países de la región
    ├── Auditoría externa de datos y cumplimiento (Ley 172-13)
    ├── Certificación de seguridad ISO 27001
    └── Transferencia tecnológica al Estado dominicano (OGTIC / DIGEIG)
```

### 3.2 Hitos y dependencias externas

| Hito | Dependencia externa | Impacto si no se concreta |
|---|---|---|
| Integración Registro Nacional (Fase 2) | Convenio con Policía Nacional + MIP | Se mantiene scraping como alternativa parcial |
| Feed hospitalario (Fase 2) | Convenio con SNS | Se mantiene registro manual por personal de salud |
| Reconocimiento facial (Fase 4) | Reforma legal / autorización judicial explícita | No se implementa sin marco legal |
| Datos abiertos (Fase 3) | Publicación en datos.gob.do (DIGEIG) | Los datos se publican en el portal propio |
| Transferencia tecnológica (Fase 5) | Designación OGTIC/DIGEIG como entidad receptora | El sistema opera desde entidad privada/ONG |

### 3.3 Estimación de costos MVP (recursos técnicos)

| Recurso | Cantidad | Costo estimado/mes | Proveedor sugerido |
|---|---|---|---|
| Servidor SQL Server (cloud) | 1 (2 vCPU, 8GB RAM, 100GB SSD) | ~$50-80 USD | Azure / AWS / Hostinger RD |
| Servidor web .NET 8 | 1 (2 vCPU, 4GB RAM) | ~$40-60 USD | Azure / AWS / Hostinger RD |
| Almacenamiento de fotos | 50GB | ~$5-10 USD | Azure Blob / AWS S3 |
| SMS API (Twilio) | ~500 SMS/mes | ~$20-40 USD | Twilio (cobertura RD confirmada) |
| Email API (SendGrid) | ~1,000 emails/mes | ~$0-20 USD | SendGrid (free tier 100/día) |
| Dominio | 1 (.com.do) | ~$30/año | NIC.do / Namecheap |
| HTTPS / TLS | 1 (Let's Encrypt) | $0 | Let's Encrypt / Cloudflare |

**Costo operativo mensual estimado MVP: ~$150-200 USD/mes**

---

## 4. GLOSARIO DE TAREAS

| Término | Definición |
|---|---|
| **DoD (Definition of Done)** | Criterios verificables que deben cumplirse para considerar una tarea completa. Incluye: código compila, tests pasan, revisión de código, sin errores conocidos. |
| **Estimación** | Horas ideales de desarrollo (sin interrupciones, sin reuniones). Multiplicar por 1.5-2x para horas reales. |
| **Seed** | Datos de prueba precargados en la base de datos para desarrollo y demo. |
| **Sintético** | Dato generado artificialmente (no corresponde a un caso real). Siempre marcado explícitamente. |

---

*Fin de la FASE 4. El plan de tareas está completo y listo para iniciar implementación.*
