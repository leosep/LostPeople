using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostPeople.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadosCaso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColorHex = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    OrdenFlujo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCaso", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FuentesDatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MetodoAcceso = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RiesgoLegal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlBase = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FormatoDatos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IntervaloMinutos = table.Column<int>(type: "int", nullable: false),
                    SelectorHtml = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConfiguracionJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    EstadoSalud = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UltimaEjecucionOk = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UltimoError = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FallosConsecutivos = table.Column<int>(type: "int", nullable: false),
                    TotalEjecuciones = table.Column<int>(type: "int", nullable: false),
                    TotalRegistrosObtenidos = table.Column<int>(type: "int", nullable: false),
                    NotasLegales = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuentesDatos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Permisos = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZonasGeograficas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ZonaPadreId = table.Column<int>(type: "int", nullable: true),
                    LatitudCentroide = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    LongitudCentroide = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZonasGeograficas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZonasGeograficas_ZonasGeograficas_ZonaPadreId",
                        column: x => x.ZonaPadreId,
                        principalTable: "ZonasGeograficas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosIngeridos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FuenteId = table.Column<int>(type: "int", nullable: false),
                    IdentificadorExterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimerNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SegundoNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PrimerApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SegundoApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Sexo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EdadAproximada = table.Column<int>(type: "int", nullable: true),
                    DescripcionFisica = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    UbicacionTexto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Latitud = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    Longitud = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    TelefonoContacto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    InstitucionOrigen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UrlOrigen = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    HtmlCrudo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoPaciente = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaRegistroFuente = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaIngesta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HashContenido = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CoincidenciaProcesada = table.Column<bool>(type: "bit", nullable: false),
                    ScoreMaximoCoincidencia = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosIngeridos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosIngeridos_FuentesDatos_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "FuentesDatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CentrosSalud",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ZonaId = table.Column<int>(type: "int", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactoEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentrosSalud", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CentrosSalud_ZonasGeograficas_ZonaId",
                        column: x => x.ZonaId,
                        principalTable: "ZonasGeograficas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonasReportadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrimerNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SegundoNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PrimerApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SegundoApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaDesaparicion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EdadAproximada = table.Column<int>(type: "int", nullable: true),
                    Sexo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TipoDocumento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Nacionalidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DescripcionFisica = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EstaturaCm = table.Column<decimal>(type: "decimal(5,1)", precision: 5, scale: 1, nullable: true),
                    PesoKg = table.Column<decimal>(type: "decimal(5,1)", precision: 5, scale: 1, nullable: true),
                    ColorPiel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ColorOjos = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ColorCabello = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SenasParticulares = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CondicionMedica = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MedicamentosRequeridos = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Vestimenta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EsMenorEdad = table.Column<bool>(type: "bit", nullable: false),
                    EsDiscapacitado = table.Column<bool>(type: "bit", nullable: false),
                    EsAdultoMayor = table.Column<bool>(type: "bit", nullable: false),
                    EsViolenciaGenero = table.Column<bool>(type: "bit", nullable: false),
                    TipoAlerta = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FotoThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FotoWebUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FotoOriginalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoCasoId = table.Column<int>(type: "int", nullable: false),
                    UltimaUbicacionZonaId = table.Column<int>(type: "int", nullable: true),
                    UltimaUbicacionLat = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    UltimaUbicacionLng = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    UltimaUbicacionTexto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CodigoSeguimiento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DatosSinteticos = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaUltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonasReportadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonasReportadas_EstadosCaso_EstadoCasoId",
                        column: x => x.EstadoCasoId,
                        principalTable: "EstadosCaso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonasReportadas_ZonasGeograficas_UltimaUbicacionZonaId",
                        column: x => x.UltimaUbicacionZonaId,
                        principalTable: "ZonasGeograficas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Cedula = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    ZonaAsignadaId = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Verificado = table.Column<bool>(type: "bit", nullable: false),
                    Institucion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Cargo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TokenVerificacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaUltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AceptoTerminos = table.Column<bool>(type: "bit", nullable: false),
                    AceptoConfidencialidad = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaUltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuarios_ZonasGeograficas_ZonaAsignadaId",
                        column: x => x.ZonaAsignadaId,
                        principalTable: "ZonasGeograficas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    Entidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntidadId = table.Column<int>(type: "int", nullable: true),
                    TipoOperacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ValorAnterior = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorNuevo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Detalles = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auditorias_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Coincidencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportePersonaId = table.Column<int>(type: "int", nullable: false),
                    RegistroIngeridoId = table.Column<int>(type: "int", nullable: false),
                    ScoreGeneral = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ScoreNombre = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ScoreEdad = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ScoreSexo = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ScoreUbicacion = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ScoreDescripcion = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    AlgoritmoUsado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Revisada = table.Column<bool>(type: "bit", nullable: false),
                    RevisorUsuarioId = table.Column<int>(type: "int", nullable: true),
                    ResultadoRevision = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    NotasRevision = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaDeteccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaRevision = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coincidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coincidencias_PersonasReportadas_ReportePersonaId",
                        column: x => x.ReportePersonaId,
                        principalTable: "PersonasReportadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Coincidencias_RegistrosIngeridos_RegistroIngeridoId",
                        column: x => x.RegistroIngeridoId,
                        principalTable: "RegistrosIngeridos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Coincidencias_Usuarios_RevisorUsuarioId",
                        column: x => x.RevisorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    ReportePersonaId = table.Column<int>(type: "int", nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Mensaje = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Leida = table.Column<bool>(type: "bit", nullable: false),
                    Enviada = table.Column<bool>(type: "bit", nullable: false),
                    CanalEnvio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaLectura = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_PersonasReportadas_ReportePersonaId",
                        column: x => x.ReportePersonaId,
                        principalTable: "PersonasReportadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reportes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonaId = table.Column<int>(type: "int", nullable: false),
                    ReportanteUsuarioId = table.Column<int>(type: "int", nullable: false),
                    RelacionConDesaparecido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TelefonoContacto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EmailContacto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodigoVerificacion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Verificado = table.Column<bool>(type: "bit", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FuenteReporte = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IpOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EsReporteLocalizacion = table.Column<bool>(type: "bit", nullable: false),
                    DetalleLocalizacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaUltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reportes_PersonasReportadas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "PersonasReportadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reportes_Usuarios_ReportanteUsuarioId",
                        column: x => x.ReportanteUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SesionesUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IpOrigen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SesionesUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SesionesUsuario_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Verificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoincidenciaId = table.Column<int>(type: "int", nullable: false),
                    VerificadorUsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoVerificacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Resultado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    MetodoContacto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DetalleContacto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaVerificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Verificaciones_Coincidencias_CoincidenciaId",
                        column: x => x.CoincidenciaId,
                        principalTable: "Coincidencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Verificaciones_Usuarios_VerificadorUsuarioId",
                        column: x => x.VerificadorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Archivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonaId = table.Column<int>(type: "int", nullable: true),
                    ReporteId = table.Column<int>(type: "int", nullable: true),
                    NombreOriginal = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TipoContenido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TamanoBytes = table.Column<long>(type: "bigint", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Publico = table.Column<bool>(type: "bit", nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubidoPorUsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Archivos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Archivos_PersonasReportadas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "PersonasReportadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Archivos_Reportes_ReporteId",
                        column: x => x.ReporteId,
                        principalTable: "Reportes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Archivos_Usuarios_SubidoPorUsuarioId",
                        column: x => x.SubidoPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Archivos_PersonaId",
                table: "Archivos",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Archivos_ReporteId",
                table: "Archivos",
                column: "ReporteId");

            migrationBuilder.CreateIndex(
                name: "IX_Archivos_SubidoPorUsuarioId",
                table: "Archivos",
                column: "SubidoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_Fecha",
                table: "Auditorias",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_UsuarioId",
                table: "Auditorias",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CentrosSalud_ZonaId",
                table: "CentrosSalud",
                column: "ZonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Coincidencias_RegistroIngeridoId",
                table: "Coincidencias",
                column: "RegistroIngeridoId");

            migrationBuilder.CreateIndex(
                name: "IX_Coincidencias_ReportePersonaId",
                table: "Coincidencias",
                column: "ReportePersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Coincidencias_Revisada",
                table: "Coincidencias",
                column: "Revisada",
                filter: "[Revisada] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Coincidencias_Revisada_ScoreGeneral",
                table: "Coincidencias",
                columns: new[] { "Revisada", "ScoreGeneral" },
                filter: "[Revisada] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Coincidencias_RevisorUsuarioId",
                table: "Coincidencias",
                column: "RevisorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Coincidencias_ScoreGeneral",
                table: "Coincidencias",
                column: "ScoreGeneral");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosCaso_Codigo",
                table: "EstadosCaso",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FuentesDatos_Codigo",
                table: "FuentesDatos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_ReportePersonaId",
                table: "Notificaciones",
                column: "ReportePersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioId_Leida_FechaCreacion",
                table: "Notificaciones",
                columns: new[] { "UsuarioId", "Leida", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonasReportadas_CodigoSeguimiento",
                table: "PersonasReportadas",
                column: "CodigoSeguimiento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonasReportadas_DatosSinteticos",
                table: "PersonasReportadas",
                column: "DatosSinteticos",
                filter: "[DatosSinteticos] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PersonasReportadas_EstadoCasoId",
                table: "PersonasReportadas",
                column: "EstadoCasoId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonasReportadas_FechaDesaparicion",
                table: "PersonasReportadas",
                column: "FechaDesaparicion");

            migrationBuilder.CreateIndex(
                name: "IX_PersonasReportadas_PrimerNombre_SegundoNombre_PrimerApellido_SegundoApellido",
                table: "PersonasReportadas",
                columns: new[] { "PrimerNombre", "SegundoNombre", "PrimerApellido", "SegundoApellido" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonasReportadas_UltimaUbicacionZonaId",
                table: "PersonasReportadas",
                column: "UltimaUbicacionZonaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosIngeridos_CoincidenciaProcesada",
                table: "RegistrosIngeridos",
                column: "CoincidenciaProcesada",
                filter: "[CoincidenciaProcesada] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosIngeridos_FuenteId_FechaRegistroFuente",
                table: "RegistrosIngeridos",
                columns: new[] { "FuenteId", "FechaRegistroFuente" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosIngeridos_HashContenido",
                table: "RegistrosIngeridos",
                column: "HashContenido",
                unique: true,
                filter: "[HashContenido] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosIngeridos_PrimerNombre_PrimerApellido",
                table: "RegistrosIngeridos",
                columns: new[] { "PrimerNombre", "PrimerApellido" });

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_PersonaId",
                table: "Reportes",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_ReportanteUsuarioId",
                table: "Reportes",
                column: "ReportanteUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SesionesUsuario_UsuarioId",
                table: "SesionesUsuario",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ZonaAsignadaId",
                table: "Usuarios",
                column: "ZonaAsignadaId");

            migrationBuilder.CreateIndex(
                name: "IX_Verificaciones_CoincidenciaId",
                table: "Verificaciones",
                column: "CoincidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Verificaciones_VerificadorUsuarioId",
                table: "Verificaciones",
                column: "VerificadorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ZonasGeograficas_Tipo",
                table: "ZonasGeograficas",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_ZonasGeograficas_ZonaPadreId",
                table: "ZonasGeograficas",
                column: "ZonaPadreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Archivos");

            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropTable(
                name: "CentrosSalud");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "SesionesUsuario");

            migrationBuilder.DropTable(
                name: "Verificaciones");

            migrationBuilder.DropTable(
                name: "Reportes");

            migrationBuilder.DropTable(
                name: "Coincidencias");

            migrationBuilder.DropTable(
                name: "PersonasReportadas");

            migrationBuilder.DropTable(
                name: "RegistrosIngeridos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "EstadosCaso");

            migrationBuilder.DropTable(
                name: "FuentesDatos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "ZonasGeograficas");
        }
    }
}
