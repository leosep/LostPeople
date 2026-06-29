IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [EstadosCaso] (
    [Id] int NOT NULL IDENTITY,
    [Codigo] nvarchar(30) NOT NULL,
    [Nombre] nvarchar(100) NOT NULL,
    [Descripcion] nvarchar(max) NULL,
    [ColorHex] nvarchar(10) NULL,
    [OrdenFlujo] int NOT NULL,
    CONSTRAINT [PK_EstadosCaso] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [FuentesDatos] (
    [Id] int NOT NULL IDENTITY,
    [Codigo] nvarchar(50) NOT NULL,
    [Nombre] nvarchar(200) NOT NULL,
    [Tipo] nvarchar(30) NOT NULL,
    [MetodoAcceso] nvarchar(max) NULL,
    [RiesgoLegal] nvarchar(max) NULL,
    [UrlBase] nvarchar(500) NULL,
    [FormatoDatos] nvarchar(max) NULL,
    [IntervaloMinutos] int NOT NULL,
    [SelectorHtml] nvarchar(500) NULL,
    [ConfiguracionJson] nvarchar(max) NULL,
    [Activo] bit NOT NULL,
    [EstadoSalud] nvarchar(50) NULL,
    [UltimaEjecucionOk] datetime2 NULL,
    [UltimoError] datetime2 NULL,
    [FallosConsecutivos] int NOT NULL,
    [TotalEjecuciones] int NOT NULL,
    [TotalRegistrosObtenidos] int NOT NULL,
    [NotasLegales] nvarchar(2000) NULL,
    CONSTRAINT [PK_FuentesDatos] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(50) NOT NULL,
    [Descripcion] nvarchar(200) NULL,
    [Permisos] nvarchar(max) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ZonasGeograficas] (
    [Id] int NOT NULL IDENTITY,
    [Codigo] nvarchar(20) NOT NULL,
    [Nombre] nvarchar(100) NOT NULL,
    [Tipo] nvarchar(20) NOT NULL,
    [ZonaPadreId] int NULL,
    [LatitudCentroide] decimal(10,7) NULL,
    [LongitudCentroide] decimal(10,7) NULL,
    CONSTRAINT [PK_ZonasGeograficas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ZonasGeograficas_ZonasGeograficas_ZonaPadreId] FOREIGN KEY ([ZonaPadreId]) REFERENCES [ZonasGeograficas] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [RegistrosIngeridos] (
    [Id] int NOT NULL IDENTITY,
    [FuenteId] int NOT NULL,
    [IdentificadorExterno] nvarchar(max) NULL,
    [PrimerNombre] nvarchar(100) NULL,
    [SegundoNombre] nvarchar(100) NULL,
    [PrimerApellido] nvarchar(100) NULL,
    [SegundoApellido] nvarchar(100) NULL,
    [Sexo] nvarchar(20) NULL,
    [EdadAproximada] int NULL,
    [DescripcionFisica] nvarchar(2000) NULL,
    [UbicacionTexto] nvarchar(500) NULL,
    [Latitud] decimal(10,7) NULL,
    [Longitud] decimal(10,7) NULL,
    [TelefonoContacto] nvarchar(20) NULL,
    [InstitucionOrigen] nvarchar(200) NULL,
    [UrlOrigen] nvarchar(1000) NULL,
    [HtmlCrudo] nvarchar(max) NULL,
    [EstadoPaciente] nvarchar(50) NULL,
    [FechaRegistroFuente] datetime2 NULL,
    [FechaIngesta] datetime2 NOT NULL,
    [HashContenido] nvarchar(64) NULL,
    [CoincidenciaProcesada] bit NOT NULL,
    [ScoreMaximoCoincidencia] int NULL,
    CONSTRAINT [PK_RegistrosIngeridos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RegistrosIngeridos_FuentesDatos_FuenteId] FOREIGN KEY ([FuenteId]) REFERENCES [FuentesDatos] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [CentrosSalud] (
    [Id] int NOT NULL IDENTITY,
    [Codigo] nvarchar(20) NOT NULL,
    [Nombre] nvarchar(200) NOT NULL,
    [Tipo] nvarchar(50) NULL,
    [ZonaId] int NOT NULL,
    [Direccion] nvarchar(500) NULL,
    [Telefono] nvarchar(20) NULL,
    [ContactoEmail] nvarchar(200) NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_CentrosSalud] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CentrosSalud_ZonasGeograficas_ZonaId] FOREIGN KEY ([ZonaId]) REFERENCES [ZonasGeograficas] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [PersonasReportadas] (
    [Id] int NOT NULL IDENTITY,
    [PrimerNombre] nvarchar(100) NOT NULL,
    [SegundoNombre] nvarchar(100) NULL,
    [PrimerApellido] nvarchar(100) NOT NULL,
    [SegundoApellido] nvarchar(100) NULL,
    [Alias] nvarchar(100) NULL,
    [FechaNacimiento] datetime2 NULL,
    [FechaDesaparicion] datetime2 NOT NULL,
    [EdadAproximada] int NULL,
    [Sexo] nvarchar(20) NULL,
    [TipoDocumento] nvarchar(30) NULL,
    [NumeroDocumento] nvarchar(30) NULL,
    [Nacionalidad] nvarchar(50) NULL,
    [DescripcionFisica] nvarchar(2000) NULL,
    [EstaturaCm] decimal(5,1) NULL,
    [PesoKg] decimal(5,1) NULL,
    [ColorPiel] nvarchar(30) NULL,
    [ColorOjos] nvarchar(30) NULL,
    [ColorCabello] nvarchar(30) NULL,
    [SenasParticulares] nvarchar(2000) NULL,
    [CondicionMedica] nvarchar(500) NULL,
    [MedicamentosRequeridos] nvarchar(500) NULL,
    [Vestimenta] nvarchar(500) NULL,
    [EsMenorEdad] bit NOT NULL,
    [EsDiscapacitado] bit NOT NULL,
    [EsAdultoMayor] bit NOT NULL,
    [EsViolenciaGenero] bit NOT NULL,
    [TipoAlerta] nvarchar(20) NULL,
    [FotoThumbnailUrl] nvarchar(max) NULL,
    [FotoWebUrl] nvarchar(max) NULL,
    [FotoOriginalUrl] nvarchar(max) NULL,
    [EstadoCasoId] int NOT NULL,
    [UltimaUbicacionZonaId] int NULL,
    [UltimaUbicacionLat] decimal(10,7) NULL,
    [UltimaUbicacionLng] decimal(10,7) NULL,
    [UltimaUbicacionTexto] nvarchar(500) NULL,
    [CodigoSeguimiento] nvarchar(20) NOT NULL,
    [DatosSinteticos] bit NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaUltimaActualizacion] datetime2 NULL,
    CONSTRAINT [PK_PersonasReportadas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PersonasReportadas_EstadosCaso_EstadoCasoId] FOREIGN KEY ([EstadoCasoId]) REFERENCES [EstadosCaso] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PersonasReportadas_ZonasGeograficas_UltimaUbicacionZonaId] FOREIGN KEY ([UltimaUbicacionZonaId]) REFERENCES [ZonasGeograficas] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Usuarios] (
    [Id] int NOT NULL IDENTITY,
    [NombreCompleto] nvarchar(200) NOT NULL,
    [Email] nvarchar(200) NOT NULL,
    [Telefono] nvarchar(20) NULL,
    [PasswordHash] nvarchar(500) NOT NULL,
    [Cedula] nvarchar(20) NULL,
    [RolId] int NOT NULL,
    [ZonaAsignadaId] int NULL,
    [Activo] bit NOT NULL,
    [Verificado] bit NOT NULL,
    [Institucion] nvarchar(200) NULL,
    [Cargo] nvarchar(200) NULL,
    [TokenVerificacion] nvarchar(100) NULL,
    [FechaUltimoAcceso] datetime2 NULL,
    [AceptoTerminos] bit NOT NULL,
    [AceptoConfidencialidad] bit NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaUltimaActualizacion] datetime2 NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Usuarios_Roles_RolId] FOREIGN KEY ([RolId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Usuarios_ZonasGeograficas_ZonaAsignadaId] FOREIGN KEY ([ZonaAsignadaId]) REFERENCES [ZonasGeograficas] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Auditorias] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NULL,
    [Entidad] nvarchar(100) NULL,
    [EntidadId] int NULL,
    [TipoOperacion] nvarchar(50) NULL,
    [ValorAnterior] nvarchar(max) NULL,
    [ValorNuevo] nvarchar(max) NULL,
    [IpOrigen] nvarchar(50) NULL,
    [UserAgent] nvarchar(500) NULL,
    [Detalles] nvarchar(max) NULL,
    [Fecha] datetime2 NOT NULL,
    CONSTRAINT [PK_Auditorias] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Auditorias_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Coincidencias] (
    [Id] int NOT NULL IDENTITY,
    [ReportePersonaId] int NOT NULL,
    [RegistroIngeridoId] int NOT NULL,
    [ScoreGeneral] decimal(5,2) NOT NULL,
    [ScoreNombre] decimal(5,2) NOT NULL,
    [ScoreEdad] decimal(5,2) NOT NULL,
    [ScoreSexo] decimal(5,2) NOT NULL,
    [ScoreUbicacion] decimal(5,2) NOT NULL,
    [ScoreDescripcion] decimal(5,2) NOT NULL,
    [AlgoritmoUsado] nvarchar(50) NULL,
    [Revisada] bit NOT NULL,
    [RevisorUsuarioId] int NULL,
    [ResultadoRevision] nvarchar(30) NULL,
    [NotasRevision] nvarchar(2000) NULL,
    [FechaDeteccion] datetime2 NOT NULL,
    [FechaRevision] datetime2 NULL,
    CONSTRAINT [PK_Coincidencias] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Coincidencias_PersonasReportadas_ReportePersonaId] FOREIGN KEY ([ReportePersonaId]) REFERENCES [PersonasReportadas] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Coincidencias_RegistrosIngeridos_RegistroIngeridoId] FOREIGN KEY ([RegistroIngeridoId]) REFERENCES [RegistrosIngeridos] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Coincidencias_Usuarios_RevisorUsuarioId] FOREIGN KEY ([RevisorUsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Notificaciones] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [ReportePersonaId] int NULL,
    [Tipo] nvarchar(50) NOT NULL,
    [Titulo] nvarchar(200) NULL,
    [Mensaje] nvarchar(2000) NULL,
    [Leida] bit NOT NULL,
    [Enviada] bit NOT NULL,
    [CanalEnvio] nvarchar(20) NULL,
    [FechaEnvio] datetime2 NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaLectura] datetime2 NULL,
    CONSTRAINT [PK_Notificaciones] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notificaciones_PersonasReportadas_ReportePersonaId] FOREIGN KEY ([ReportePersonaId]) REFERENCES [PersonasReportadas] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Notificaciones_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Reportes] (
    [Id] int NOT NULL IDENTITY,
    [PersonaId] int NOT NULL,
    [ReportanteUsuarioId] int NOT NULL,
    [RelacionConDesaparecido] nvarchar(100) NULL,
    [TelefonoContacto] nvarchar(20) NULL,
    [EmailContacto] nvarchar(200) NULL,
    [CodigoVerificacion] nvarchar(10) NULL,
    [Verificado] bit NOT NULL,
    [Notas] nvarchar(2000) NULL,
    [FuenteReporte] nvarchar(50) NULL,
    [IpOrigen] nvarchar(50) NULL,
    [EsReporteLocalizacion] bit NOT NULL,
    [DetalleLocalizacion] nvarchar(max) NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaUltimaActualizacion] datetime2 NULL,
    CONSTRAINT [PK_Reportes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reportes_PersonasReportadas_PersonaId] FOREIGN KEY ([PersonaId]) REFERENCES [PersonasReportadas] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Reportes_Usuarios_ReportanteUsuarioId] FOREIGN KEY ([ReportanteUsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [SesionesUsuario] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [Token] nvarchar(500) NOT NULL,
    [RefreshToken] nvarchar(500) NULL,
    [IpOrigen] nvarchar(50) NULL,
    [UserAgent] nvarchar(500) NULL,
    [FechaInicio] datetime2 NOT NULL,
    [FechaExpiracion] datetime2 NULL,
    [Activa] bit NOT NULL,
    CONSTRAINT [PK_SesionesUsuario] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SesionesUsuario_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Verificaciones] (
    [Id] int NOT NULL IDENTITY,
    [CoincidenciaId] int NOT NULL,
    [VerificadorUsuarioId] int NOT NULL,
    [TipoVerificacion] nvarchar(50) NULL,
    [Resultado] nvarchar(30) NULL,
    [MetodoContacto] nvarchar(50) NULL,
    [DetalleContacto] nvarchar(500) NULL,
    [Notas] nvarchar(2000) NULL,
    [FechaVerificacion] datetime2 NOT NULL,
    CONSTRAINT [PK_Verificaciones] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Verificaciones_Coincidencias_CoincidenciaId] FOREIGN KEY ([CoincidenciaId]) REFERENCES [Coincidencias] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Verificaciones_Usuarios_VerificadorUsuarioId] FOREIGN KEY ([VerificadorUsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Archivos] (
    [Id] int NOT NULL IDENTITY,
    [PersonaId] int NULL,
    [ReporteId] int NULL,
    [NombreOriginal] nvarchar(500) NOT NULL,
    [RutaArchivo] nvarchar(1000) NOT NULL,
    [TipoContenido] nvarchar(100) NULL,
    [TamanoBytes] bigint NOT NULL,
    [Categoria] nvarchar(50) NULL,
    [Publico] bit NOT NULL,
    [FechaSubida] datetime2 NOT NULL,
    [SubidoPorUsuarioId] int NOT NULL,
    CONSTRAINT [PK_Archivos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Archivos_PersonasReportadas_PersonaId] FOREIGN KEY ([PersonaId]) REFERENCES [PersonasReportadas] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Archivos_Reportes_ReporteId] FOREIGN KEY ([ReporteId]) REFERENCES [Reportes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Archivos_Usuarios_SubidoPorUsuarioId] FOREIGN KEY ([SubidoPorUsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Archivos_PersonaId] ON [Archivos] ([PersonaId]);
GO

CREATE INDEX [IX_Archivos_ReporteId] ON [Archivos] ([ReporteId]);
GO

CREATE INDEX [IX_Archivos_SubidoPorUsuarioId] ON [Archivos] ([SubidoPorUsuarioId]);
GO

CREATE INDEX [IX_Auditorias_Fecha] ON [Auditorias] ([Fecha]);
GO

CREATE INDEX [IX_Auditorias_UsuarioId] ON [Auditorias] ([UsuarioId]);
GO

CREATE INDEX [IX_CentrosSalud_ZonaId] ON [CentrosSalud] ([ZonaId]);
GO

CREATE INDEX [IX_Coincidencias_RegistroIngeridoId] ON [Coincidencias] ([RegistroIngeridoId]);
GO

CREATE INDEX [IX_Coincidencias_ReportePersonaId] ON [Coincidencias] ([ReportePersonaId]);
GO

CREATE INDEX [IX_Coincidencias_Revisada] ON [Coincidencias] ([Revisada]) WHERE [Revisada] = 0;
GO

CREATE INDEX [IX_Coincidencias_Revisada_ScoreGeneral] ON [Coincidencias] ([Revisada], [ScoreGeneral]) WHERE [Revisada] = 0;
GO

CREATE INDEX [IX_Coincidencias_RevisorUsuarioId] ON [Coincidencias] ([RevisorUsuarioId]);
GO

CREATE INDEX [IX_Coincidencias_ScoreGeneral] ON [Coincidencias] ([ScoreGeneral]);
GO

CREATE UNIQUE INDEX [IX_EstadosCaso_Codigo] ON [EstadosCaso] ([Codigo]);
GO

CREATE UNIQUE INDEX [IX_FuentesDatos_Codigo] ON [FuentesDatos] ([Codigo]);
GO

CREATE INDEX [IX_Notificaciones_ReportePersonaId] ON [Notificaciones] ([ReportePersonaId]);
GO

CREATE INDEX [IX_Notificaciones_UsuarioId_Leida_FechaCreacion] ON [Notificaciones] ([UsuarioId], [Leida], [FechaCreacion]);
GO

CREATE UNIQUE INDEX [IX_PersonasReportadas_CodigoSeguimiento] ON [PersonasReportadas] ([CodigoSeguimiento]);
GO

CREATE INDEX [IX_PersonasReportadas_DatosSinteticos] ON [PersonasReportadas] ([DatosSinteticos]) WHERE [DatosSinteticos] = 0;
GO

CREATE INDEX [IX_PersonasReportadas_EstadoCasoId] ON [PersonasReportadas] ([EstadoCasoId]);
GO

CREATE INDEX [IX_PersonasReportadas_FechaDesaparicion] ON [PersonasReportadas] ([FechaDesaparicion]);
GO

CREATE INDEX [IX_PersonasReportadas_PrimerNombre_SegundoNombre_PrimerApellido_SegundoApellido] ON [PersonasReportadas] ([PrimerNombre], [SegundoNombre], [PrimerApellido], [SegundoApellido]);
GO

CREATE INDEX [IX_PersonasReportadas_UltimaUbicacionZonaId] ON [PersonasReportadas] ([UltimaUbicacionZonaId]);
GO

CREATE INDEX [IX_RegistrosIngeridos_CoincidenciaProcesada] ON [RegistrosIngeridos] ([CoincidenciaProcesada]) WHERE [CoincidenciaProcesada] = 0;
GO

CREATE INDEX [IX_RegistrosIngeridos_FuenteId_FechaRegistroFuente] ON [RegistrosIngeridos] ([FuenteId], [FechaRegistroFuente]);
GO

CREATE UNIQUE INDEX [IX_RegistrosIngeridos_HashContenido] ON [RegistrosIngeridos] ([HashContenido]) WHERE [HashContenido] IS NOT NULL;
GO

CREATE INDEX [IX_RegistrosIngeridos_PrimerNombre_PrimerApellido] ON [RegistrosIngeridos] ([PrimerNombre], [PrimerApellido]);
GO

CREATE INDEX [IX_Reportes_PersonaId] ON [Reportes] ([PersonaId]);
GO

CREATE INDEX [IX_Reportes_ReportanteUsuarioId] ON [Reportes] ([ReportanteUsuarioId]);
GO

CREATE UNIQUE INDEX [IX_Roles_Nombre] ON [Roles] ([Nombre]);
GO

CREATE INDEX [IX_SesionesUsuario_UsuarioId] ON [SesionesUsuario] ([UsuarioId]);
GO

CREATE UNIQUE INDEX [IX_Usuarios_Email] ON [Usuarios] ([Email]);
GO

CREATE INDEX [IX_Usuarios_RolId] ON [Usuarios] ([RolId]);
GO

CREATE INDEX [IX_Usuarios_ZonaAsignadaId] ON [Usuarios] ([ZonaAsignadaId]);
GO

CREATE INDEX [IX_Verificaciones_CoincidenciaId] ON [Verificaciones] ([CoincidenciaId]);
GO

CREATE INDEX [IX_Verificaciones_VerificadorUsuarioId] ON [Verificaciones] ([VerificadorUsuarioId]);
GO

CREATE INDEX [IX_ZonasGeograficas_Tipo] ON [ZonasGeograficas] ([Tipo]);
GO

CREATE INDEX [IX_ZonasGeograficas_ZonaPadreId] ON [ZonasGeograficas] ([ZonaPadreId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260629125944_InitialCreate', N'8.0.28');
GO

COMMIT;
GO

