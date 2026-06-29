using LostPeople.Domain.Entities;
using LostPeople.Infrastructure.Services;

namespace LostPeople.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(LostPeopleDbContext context)
    {
        if (context.EstadosCaso.Any()) return;

        var estados = new List<EstadoCaso>
        {
            new() { Codigo = "RECIBIDO", Nombre = "Recibido", ColorHex = "#3B82F6", OrdenFlujo = 1 },
            new() { Codigo = "VERIFICACION", Nombre = "En verificación", ColorHex = "#EAB308", OrdenFlujo = 2 },
            new() { Codigo = "COINCIDENCIA", Nombre = "Coincidencia detectada", ColorHex = "#F97316", OrdenFlujo = 3 },
            new() { Codigo = "INVESTIGACION", Nombre = "En investigación", ColorHex = "#A855F7", OrdenFlujo = 4 },
            new() { Codigo = "LOCALIZADO_VIVO", Nombre = "Localizado con vida", ColorHex = "#22C55E", OrdenFlujo = 5 },
            new() { Codigo = "LOCALIZADO_FALLECIDO", Nombre = "Localizado (fallecido)", ColorHex = "#6B7280", OrdenFlujo = 6 },
            new() { Codigo = "CERRADO", Nombre = "Cerrado sin resolver", ColorHex = "#EF4444", OrdenFlujo = 7 }
        };
        context.EstadosCaso.AddRange(estados);

        var roles = new List<Rol>
        {
            new() { Nombre = "Ciudadano", Descripcion = "Usuario ciudadano general" },
            new() { Nombre = "Familiar", Descripcion = "Familiar o tutor de persona reportada" },
            new() { Nombre = "Voluntario", Descripcion = "Voluntario verificador" },
            new() { Nombre = "PersonalSalud", Descripcion = "Personal hospitalario" },
            new() { Nombre = "AgentePolicial", Descripcion = "Agente de la Policía Nacional" },
            new() { Nombre = "Admin", Descripcion = "Administrador de plataforma" },
            new() { Nombre = "SuperAdmin", Descripcion = "Superadmin gubernamental" }
        };
        context.Roles.AddRange(roles);

        var provincias = new List<ZonaGeografica>
        {
            new() { Codigo = "DN", Nombre = "Distrito Nacional", Tipo = "Provincia" },
            new() { Codigo = "AZUA", Nombre = "Azua", Tipo = "Provincia" },
            new() { Codigo = "BAORUCO", Nombre = "Baoruco", Tipo = "Provincia" },
            new() { Codigo = "BARAHONA", Nombre = "Barahona", Tipo = "Provincia" },
            new() { Codigo = "DAJABON", Nombre = "Dajabón", Tipo = "Provincia" },
            new() { Codigo = "DUARTE", Nombre = "Duarte", Tipo = "Provincia" },
            new() { Codigo = "EL_SEIBO", Nombre = "El Seibo", Tipo = "Provincia" },
            new() { Codigo = "ELIAS_PINA", Nombre = "Elías Piña", Tipo = "Provincia" },
            new() { Codigo = "ESPAILLAT", Nombre = "Espaillat", Tipo = "Provincia" },
            new() { Codigo = "HATO_MAYOR", Nombre = "Hato Mayor", Tipo = "Provincia" },
            new() { Codigo = "HERMANAS_MIRABAL", Nombre = "Hermanas Mirabal", Tipo = "Provincia" },
            new() { Codigo = "INDEPENDENCIA", Nombre = "Independencia", Tipo = "Provincia" },
            new() { Codigo = "LA_ALTAGRACIA", Nombre = "La Altagracia", Tipo = "Provincia" },
            new() { Codigo = "LA_ROMANA", Nombre = "La Romana", Tipo = "Provincia" },
            new() { Codigo = "LA_VEGA", Nombre = "La Vega", Tipo = "Provincia" },
            new() { Codigo = "MARIA_T_SANCHEZ", Nombre = "María Trinidad Sánchez", Tipo = "Provincia" },
            new() { Codigo = "MONSEÑOR_NOUEL", Nombre = "Monseñor Nouel", Tipo = "Provincia" },
            new() { Codigo = "MONTE_CRISTI", Nombre = "Monte Cristi", Tipo = "Provincia" },
            new() { Codigo = "MONTE_PLATA", Nombre = "Monte Plata", Tipo = "Provincia" },
            new() { Codigo = "PEDERNALES", Nombre = "Pedernales", Tipo = "Provincia" },
            new() { Codigo = "PERAVIA", Nombre = "Peravia", Tipo = "Provincia" },
            new() { Codigo = "PUERTO_PLATA", Nombre = "Puerto Plata", Tipo = "Provincia" },
            new() { Codigo = "SAMANA", Nombre = "Samaná", Tipo = "Provincia" },
            new() { Codigo = "SAN_CRISTOBAL", Nombre = "San Cristóbal", Tipo = "Provincia" },
            new() { Codigo = "SAN_JOSE_DE_OCOA", Nombre = "San José de Ocoa", Tipo = "Provincia" },
            new() { Codigo = "SAN_JUAN", Nombre = "San Juan", Tipo = "Provincia" },
            new() { Codigo = "SAN_PEDRO_DE_MACORIS", Nombre = "San Pedro de Macorís", Tipo = "Provincia" },
            new() { Codigo = "SANCHEZ_RAMIREZ", Nombre = "Sánchez Ramírez", Tipo = "Provincia" },
            new() { Codigo = "SANTIAGO", Nombre = "Santiago", Tipo = "Provincia" },
            new() { Codigo = "SANTIAGO_RODRIGUEZ", Nombre = "Santiago Rodríguez", Tipo = "Provincia" },
            new() { Codigo = "SANTO_DOMINGO", Nombre = "Santo Domingo", Tipo = "Provincia" },
            new() { Codigo = "VALVERDE", Nombre = "Valverde", Tipo = "Provincia" }
        };
        context.ZonasGeograficas.AddRange(provincias);

        var fuentes = new List<FuenteDatos>
        {
            new()
            {
                Codigo = "PN_CRITICAL_MISSING",
                Nombre = "Policía Nacional - Personas Desaparecidas",
                Tipo = "API",
                MetodoAcceso = "HTTP_GET",
                UrlBase = "https://api.policianacional.gob.do/v1/personas-desaparecidas",
                FormatoDatos = "JSON",
                IntervaloMinutos = 30,
                RiesgoLegal = "MEDIO - Datos oficiales, verificar términos de uso",
                Activo = true,
                EstadoSalud = "Desconocido",
                FallosConsecutivos = 0,
                TotalEjecuciones = 0,
                TotalRegistrosObtenidos = 0,
                NotasLegales = "Requiere convenio interinstitucional. Datos sujetos a verificación."
            },
            new()
            {
                Codigo = "DATOS_GOB_DO",
                Nombre = "Datos Abiertos RD",
                Tipo = "API",
                MetodoAcceso = "HTTP_GET",
                UrlBase = "https://datos.gob.do/api/3/action/datastore_search",
                FormatoDatos = "JSON",
                IntervaloMinutos = 60,
                RiesgoLegal = "BAJO - Datos abiertos gubernamentales",
                Activo = true,
                EstadoSalud = "Desconocido",
                FallosConsecutivos = 0,
                TotalEjecuciones = 0,
                TotalRegistrosObtenidos = 0,
                NotasLegales = "Fuente de datos abiertos. Verificar licencia."
            },
            new()
            {
                Codigo = "HOSPITAL_SIMULADO",
                Nombre = "Hospitales - Simulación Demo",
                Tipo = "SIMULADO",
                MetodoAcceso = "SIMULADO",
                FormatoDatos = "JSON",
                IntervaloMinutos = 120,
                RiesgoLegal = "ALTO - Datos simulados, no usar en producción real",
                Activo = true,
                EstadoSalud = "Saludable",
                FallosConsecutivos = 0,
                TotalEjecuciones = 0,
                TotalRegistrosObtenidos = 0,
                NotasLegales = "SOLO DEMO. Contiene datos sintéticos. No reemplaza integración real con SNS."
            },
            new()
            {
                Codigo = "911_EMERGENCIAS",
                Nombre = "Sistema 9-1-1",
                Tipo = "API",
                MetodoAcceso = "HTTP_GET",
                UrlBase = "https://api.911.gob.do/v1/reportes",
                FormatoDatos = "JSON",
                IntervaloMinutos = 15,
                RiesgoLegal = "ALTO - Datos sensibles de emergencias",
                Activo = false,
                EstadoSalud = "No configurado",
                FallosConsecutivos = 0,
                TotalEjecuciones = 0,
                TotalRegistrosObtenidos = 0,
                NotasLegales = "Requiere convenio con el Sistema Nacional de Atención a Emergencias 9-1-1. Cifrado obligatorio."
            },
            new()
            {
                Codigo = "SNC_HOSPITALARIO",
                Nombre = "SNS - Pacientes NN",
                Tipo = "API",
                MetodoAcceso = "HTTP_GET",
                FormatoDatos = "JSON",
                IntervaloMinutos = 30,
                RiesgoLegal = "MUY ALTO - Datos de salud, Ley 172-13, Ley General de Salud",
                Activo = false,
                EstadoSalud = "No configurado",
                FallosConsecutivos = 0,
                TotalEjecuciones = 0,
                TotalRegistrosObtenidos = 0,
                NotasLegales = "Requiere convenio con el Servicio Nacional de Salud. Datos de salud requieren cifrado AES-256."
            }
        };
        context.FuentesDatos.AddRange(fuentes);

        if (!context.Usuarios.Any(u => u.Email == "admin@lostpeople.do"))
        {
            context.Usuarios.Add(new Usuario
            {
                NombreCompleto = "Administrador",
                Email = "admin@lostpeople.do",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                RolId = 6,
                Activo = true,
                Verificado = true,
                FechaCreacion = DateTime.UtcNow,
                AceptoTerminos = true,
                AceptoConfidencialidad = true
            });
        }

        if (!context.Usuarios.Any(u => u.Email == "verificador@lostpeople.do"))
        {
            context.Usuarios.Add(new Usuario
            {
                NombreCompleto = "Verificador Demo",
                Email = "verificador@lostpeople.do",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Verif123!"),
                RolId = 3,
                Activo = true,
                Verificado = true,
                FechaCreacion = DateTime.UtcNow,
                AceptoTerminos = true,
                AceptoConfidencialidad = true
            });
        }

        if (!context.PersonasReportadas.Any())
        {
            var provinciasList = context.ZonasGeograficas.Where(z => z.Tipo == "Provincia").ToList();
            var rnd = new Random(42);

            var personasDemo = new List<PersonaReportada>
            {
                new()
                {
                    PrimerNombre = "Maria", SegundoNombre = "Elena", PrimerApellido = "Rodriguez", SegundoApellido = "Perez",
                    EdadAproximada = 8, Sexo = "Femenino", EsMenorEdad = true,
                    DescripcionFisica = "Nina de 8 anos, cabello castano largo, ojos marrones",
                    ColorPiel = "Morena clara", ColorOjos = "Marron", ColorCabello = "Castano",
                    Vestimenta = "Uniforme escolar: camisa blanca y falda azul",
                    UltimaUbicacionTexto = "C/ Duarte esq. Palo Hincado, Villa Consuelo, DN",
                    TipoAlerta = "Amber",
                    CodigoSeguimiento = CodigoGenerator.GenerarCodigoSeguimiento(),
                    EstadoCasoId = 1,
                    DatosSinteticos = true,
                    FechaDesaparicion = DateTime.UtcNow.AddDays(-2),
                    FechaCreacion = DateTime.UtcNow.AddDays(-2)
                },
                new()
                {
                    PrimerNombre = "Carlos", PrimerApellido = "Martinez", SegundoApellido = "Lopez",
                    EdadAproximada = 45, Sexo = "Masculino",
                    DescripcionFisica = "Hombre de 45 anos, 1.75m, complexion robusta, barba canosa",
                    EstaturaCm = 175, ColorPiel = "Moreno", ColorOjos = "Marron oscuro", ColorCabello = "Negro con canas",
                    CondicionMedica = "Diabetes tipo 2, requiere insulina",
                    Vestimenta = "Camisa azul de trabajo, pantalon beige, botas negras",
                    UltimaUbicacionTexto = "Av. Abraham Lincoln esq. John F. Kennedy, Santo Domingo",
                    TipoAlerta = "Plata",
                    CodigoSeguimiento = CodigoGenerator.GenerarCodigoSeguimiento(),
                    EstadoCasoId = 2,
                    DatosSinteticos = true,
                    FechaDesaparicion = DateTime.UtcNow.AddDays(-5),
                    FechaCreacion = DateTime.UtcNow.AddDays(-5)
                },
                new()
                {
                    PrimerNombre = "Ana", SegundoNombre = "Patricia", PrimerApellido = "Castillo",
                    EdadAproximada = 22, Sexo = "Femenino",
                    DescripcionFisica = "Mujer joven, 1.60m, contextura delgada, cabello largo negro",
                    EstaturaCm = 160, ColorPiel = "Morena", ColorOjos = "Marron", ColorCabello = "Negro",
                    SenasParticulares = "Lunar en mejilla derecha, aretes de perla",
                    CondicionMedica = "Ansiedad, medicacion controlada",
                    Vestimenta = "Vestido floreado azul y blanco, sandalias",
                    UltimaUbicacionTexto = "Parque Central, Santiago de los Caballeros",
                    TipoAlerta = "Azul",
                    CodigoSeguimiento = CodigoGenerator.GenerarCodigoSeguimiento(),
                    EstadoCasoId = 1,
                    DatosSinteticos = true,
                    FechaDesaparicion = DateTime.UtcNow.AddDays(-1),
                    FechaCreacion = DateTime.UtcNow.AddDays(-1)
                },
                new()
                {
                    PrimerNombre = "Jose", PrimerApellido = "Almonte",
                    EdadAproximada = 67, Sexo = "Masculino",
                    DescripcionFisica = "Adulto mayor, 1.68m, cabello canoso corto, usa lentes",
                    EstaturaCm = 168, ColorPiel = "Moreno claro", ColorOjos = "Gris", ColorCabello = "Canoso",
                    CondicionMedica = "Alzheimer en etapa temprana, desorientacion frecuente",
                    MedicamentosRequeridos = "Donepezilo 10mg diario",
                    Vestimenta = "Guayabera blanca, pantalon beige, zapatos negros",
                    UltimaUbicacionTexto = "Plaza Central, Av. 27 de Febrero, Santo Domingo",
                    TipoAlerta = "Plata",
                    CodigoSeguimiento = CodigoGenerator.GenerarCodigoSeguimiento(),
                    EstadoCasoId = 3,
                    DatosSinteticos = true,
                    FechaDesaparicion = DateTime.UtcNow.AddDays(-7),
                    FechaCreacion = DateTime.UtcNow.AddDays(-7)
                },
                new()
                {
                    PrimerNombre = "Luisa", SegundoNombre = "Maria", PrimerApellido = "Fernandez",
                    EdadAproximada = 15, Sexo = "Femenino", EsMenorEdad = true,
                    DescripcionFisica = "Adolescente, 1.55m, cabello castano claro con reflejos, ojos verdes",
                    EstaturaCm = 155, ColorPiel = "Blanca", ColorOjos = "Verdes", ColorCabello = "Castano claro",
                    SenasParticulares = "Piercing en nariz, tatuaje pequeno de estrella en muneca",
                    Vestimenta = "Chamarra de jean, camiseta negra, leggings grises, tenis blancos",
                    UltimaUbicacionTexto = "Megacentro, Av. 27 de Febrero, Santo Domingo",
                    TipoAlerta = "Rosa",
                    CodigoSeguimiento = CodigoGenerator.GenerarCodigoSeguimiento(),
                    EstadoCasoId = 5,
                    DatosSinteticos = true,
                    FechaDesaparicion = DateTime.UtcNow.AddDays(-30),
                    FechaUltimaActualizacion = DateTime.UtcNow.AddDays(-1),
                    FechaCreacion = DateTime.UtcNow.AddDays(-30)
                },
                new()
                {
                    PrimerNombre = "Ramon", PrimerApellido = "Pena",
                    EdadAproximada = 35, Sexo = "Masculino",
                    DescripcionFisica = "Hombre de 35 anos, 1.80m, atletico, cabello rapado, tatuajes ambos brazos",
                    EstaturaCm = 180, ColorPiel = "Moreno oscuro", ColorOjos = "Negros", ColorCabello = "Rapado",
                    SenasParticulares = "Tatuaje de dragon en brazo derecho, cicatriz en menton",
                    Vestimenta = "Franela negra, jeans azules, gorra roja, tenis negros",
                    UltimaUbicacionTexto = "Av. Duarte esq. Paris, Zona Colonial, Santo Domingo",
                    TipoAlerta = "Plata",
                    CodigoSeguimiento = CodigoGenerator.GenerarCodigoSeguimiento(),
                    EstadoCasoId = 1,
                    DatosSinteticos = true,
                    FechaDesaparicion = DateTime.UtcNow.AddDays(-3),
                    FechaCreacion = DateTime.UtcNow.AddDays(-3)
                },
                new()
                {
                    PrimerNombre = "Carmen", SegundoNombre = "Rosa", PrimerApellido = "Jimenez", SegundoApellido = "Diaz",
                    EdadAproximada = 58, Sexo = "Femenino",
                    DescripcionFisica = "Mujer de 58 anos, 1.62m, cabello canoso largo recogido, usa lentes",
                    EstaturaCm = 162, ColorPiel = "Morena", ColorOjos = "Marron", ColorCabello = "Canoso",
                    SenasParticulares = "Usa pañuelo en la cabeza, aretes de cruz",
                    CondicionMedica = "Hipertension controlada con medicacion",
                    Vestimenta = "Vestido de flores, reboso negro, zapatos planos",
                    UltimaUbicacionTexto = "Mercado Modelo, Av. Mella, Zona Colonial, Santo Domingo",
                    TipoAlerta = "Plata",
                    CodigoSeguimiento = CodigoGenerator.GenerarCodigoSeguimiento(),
                    EstadoCasoId = 2,
                    DatosSinteticos = true,
                    FechaDesaparicion = DateTime.UtcNow.AddDays(-10),
                    FechaCreacion = DateTime.UtcNow.AddDays(-10)
                }
            };

            for (int i = 0; i < personasDemo.Count; i++)
            {
                var p = personasDemo[i];
                if (provinciasList.Count > 0)
                    p.UltimaUbicacionZonaId = provinciasList[i % provinciasList.Count].Id;
                p.FechaNacimiento = p.EdadAproximada.HasValue
                    ? DateTime.UtcNow.AddYears(-p.EdadAproximada.Value)
                    : null;
            }

            context.PersonasReportadas.AddRange(personasDemo);
            await context.SaveChangesAsync();

            foreach (var p in personasDemo)
            {
                context.Reportes.Add(new Reporte
                {
                    PersonaId = p.Id,
                    ReportanteUsuarioId = 1,
                    RelacionConDesaparecido = "Familiar",
                    TelefonoContacto = "809-555-0101",
                    EmailContacto = "familiar@ejemplo.com",
                    CodigoVerificacion = CodigoGenerator.GenerarCodigoVerificacion(),
                    Verificado = true,
                    FuenteReporte = "Web",
                    FechaCreacion = p.FechaCreacion
                });
            }
        }

        await context.SaveChangesAsync();
    }
}