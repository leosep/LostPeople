# FASE 0 — MAPA DE SKILLS / CONOCIMIENTO BASE

> Documento de referencia que declara los cuerpos de conocimiento especializado que se activarán a lo largo del proyecto. Cada skill tiene un nombre, una descripción de cuándo se aplica, y las fases/módulos donde es relevante.

---

## S-01 — Ley 172-13 de Protección de Datos Personales (RD)
**Cuándo se activa:** Todo diseño de almacenamiento, tratamiento y exposición de datos personales de personas reportadas, familiares, testigos y verificadores.
**Fases/Módulos:** FASE-1 (restricciones legales), FASE-2 (modelo de datos, seguridad), Módulos 1, 2, 5, 10, 11.
**Detalle:** Ley 172-13 (dic. 2013) rige el tratamiento de datos personales en RD. Exige consentimiento informado, principio de finalidad, calidad, licitud, lealtad, seguridad y deber de secreto. Los datos sensibles (salud, origen racial, orientación sexual, creencias) requieren protección reforzada y consentimiento expreso y escrito. El art. 44 constitucional y el hábeas data (art. 17) son el marco de fondo.

## S-02 — Ley 25-26 "Alertas RD" — Sistema Nacional de Búsqueda de Personas Desaparecidas
**Cuándo se activa:** Alineación estratégica de la plataforma con el marco legal recién promulgado (junio 2026). Diseño del flujo de reportes, definición de interoperabilidad con el Registro Nacional de Personas Desaparecidas y los tipos de alerta (Amber, Silver, Azul, Rosa).
**Fases/Módulos:** FASE-1 (alineación con política pública), FASE-2 (arquitectura de integración), Módulos 1, 3, 5, 12.
**Detalle:** La Ley 25-26 crea el Registro Nacional de Personas Desaparecidas (Policía Nacional), el Consejo Nacional (MIP, PGR, MD, PN, MMujer, 911, Indotel, Conani, etc.) y activación obligatoria en <24h. La plataforma debe complementar, no duplicar ni sustituir este sistema.

## S-03 — UX para usuarios en crisis emocional / trauma-informed design
**Cuándo se activa:** Diseño de todas las interfaces orientadas a familiares buscando a un desaparecido. El usuario puede estar en estado de ansiedad, duelo anticipado, desesperación o pánico.
**Fases/Módulos:** FASE-3 (UX/UI spec), Módulos 1, 2, 6, 12.
**Detalle:** Copywriting empático sin tecnicismos, flujos de una sola pregunta por paso (no formularios extensos de golpe), feedback inmediato, zero tolerance a lenguaje que implique "rendirse" o "esperar", estados de carga con mensajes de esperanza controlada, confirmación visible de que "su reporte ya está siendo procesado".

## S-04 — Web scraping resiliente con restricciones legales y éticas
**Cuándo se activa:** Diseño e implementación del Módulo 3 (Ingesta desde fuentes públicas abiertas). Evaluación legal de cada fuente.
**Fases/Módulos:** FASE-2 (evaluación legal y técnica de fuentes), Módulo 3 (pipeline de scraping).
**Detalle:** Técnicas de scraping compatibles con ToS, rate limiting respetuoso (retroceso exponencial, jitter), rotación de user-agents, adaptadores estructurales por fuente, manejo de HTML cambiante (selectores resilientes + monitoreo de rotura), logging de procedencia, almacenamiento crudo + transformado. Diferenciación entre datos públicos (ya publicados activamente por la institución) y datos que requieren convenio.

## S-05 — Fuzzy matching y deduplicación de entidades (personas)
**Cuándo se activa:** Diseño del motor de coincidencias (Módulo 4). Cruce de reportes ciudadanos vs. registros ingeridos de hospitales/policía.
**Fases/Módulos:** FASE-2 (algoritmo de matching), Módulo 4.
**Detalle:** Algoritmos de similitud de cadenas (Levenshtein, Jaro-Winkler, Soundex para nombres hispanos), normalización de nombres compuestos dominicanos, manejo de variaciones ortográficas, ponderación por campos (nombre+edad+ubicación > solo nombre), blocking/particionado para eficiencia en cruces grandes, threshold ajustable.

## S-06 — Arquitectura Clean Architecture .NET 8 + EF Core + Razor
**Cuándo se activa:** Diseño e implementación de toda la infraestructura técnica en FASE-2 y desarrollo de código.
**Fases/Módulos:** FASE-2 (arquitectura técnica, modelo de datos), todos los módulos de implementación.
**Detalle:** Separación Domain/Application/Infrastructure/Presentation. Repository + Unit of Work. Code First + Migrations. Razor Views + Tailwind. Hosted Services / Quartz.NET. Inyección de dependencias. Validación con FluentValidation. Manejo de errores consistente.

## S-07 — Prevención de fraudes y reportes maliciosos en sistemas cívicos
**Cuándo se activa:** Diseño de validación de reportes ciudadanos para evitar falsos positivos, suplantación de identidad de desaparecidos, reportes de broma o malintencionados.
**Fases/Módulos:** FASE-2 (seguridad, validación), Módulos 1, 5.
**Detalle:** Rate limiting por IP/dispositivo, verificación por SMS/correo del reportante, límite de reportes por persona en periodo corto, verificación escalonada (humano revisa coincidencias), reputación del reportante, marca de agua en fotos subidas, bloqueo de cuentas sospechosas.

## S-08 — Datos abiertos gubernamentales y Política Nacional de Datos Abiertos (RD)
**Cuándo se activa:** Evaluación de fuentes oficiales scrapeables, diseño de integración con datos.gob.do, estrategia de publicación de datos abiertos de la plataforma.
**Fases/Módulos:** FASE-2 (fuentes de datos), Módulos 3, 11.
**Detalle:** El Portal Nacional de Datos Abiertos (datos.gob.do) y la Política Nacional de Datos Abiertos (PNDA-RD) establecen lineamientos para publicación de datos públicos. La DIGEIG es el ente rector. La plataforma debe poder consumir y eventualmente producir conjuntos de datos abiertos.

## S-09 — Protección de datos de menores y personas vulnerables
**Cuándo se activa:** Manejo de reportes de niños/niñas/adolescentes (Ley 25-26: Alerta Amber), adultos mayores (Alerta Silver), personas con discapacidad (Alerta Azul), mujeres en contexto de violencia (Alerta Rosa).
**Fases/Módulos:** FASE-1 (restricciones éticas), FASE-2 (modelo de datos, permisos), Módulos 1, 2, 5, 10.
**Detalle:** La Ley 172-13 exige protección reforzada para datos de menores. La Ley 136-03 (Código para el Sistema de Protección de Menores) complementa. Las fotos y datos de menores desaparecidos requieren tratamiento especialmente cuidadoso: no exponer información que pueda facilitar su localización por terceros no autorizados o ponerlos en riesgo adicional.

## S-10 — Diseño responsive-first con Tailwind CSS y accesibilidad WCAG AA
**Cuándo se activa:** Implementación frontend de todas las vistas Razor.
**Fases/Módulos:** FASE-3 (UX/UI spec), Módulos 1, 2, 6, 7, 8, 12.
**Detalle:** Mobile-first obligatorio (muchos familiares accederán desde teléfonos). Alto contraste. WCAG AA incluyendo: navegación por teclado, etiquetas ARIA, textos alternativos, contraste 4.5:1 mínimo, tamaño de fuente redimensionable, soporte de lectores de pantalla.

## S-11 — Mecanismos de geolocalización y mapeo por zonas geográficas (RD)
**Cuándo se activa:** Captura de última ubicación conocida, mapa de casos activos por provincia/municipio (Módulo 7), filtros por zona (Módulo 6).
**Fases/Módulos:** FASE-2 (modelo geográfico), Módulos 1, 6, 7.
**Detalle:** División administrativa de RD: 31 provincias + Distrito Nacional, 158 municipios. Coordenadas (lat/lng) opcionales para última ubicación. Geocodificación inversa: texto de ubicación → coordenadas. Mapa con Leaflet/Mapbox (sin costo elevado). Marcadores con estado del caso.

## S-12 — Background jobs y procesamiento periódico (Quartz.NET / Hosted Services)
**Cuándo se activa:** Programación de scrapings periódicos, re-ejecución del motor de matching, limpieza de datos temporales, expiración de alertas.
**Fases/Módulos:** FASE-2 (arquitectura de ingestión), Módulos 3, 4, 10.
**Detalle:** Quartz.NET para jobs con cron schedules. Separación de concerns: el scheduler solo orquesta, la lógica de negocio vive en Application layer. Manejo de fallos con reintentos configurables y notificación al admin. Logging estructurado de cada ejecución.

---

*Fin del mapa de skills. Cada skill será referenciada por su código (S-01 a S-12) en las fases posteriores.*
