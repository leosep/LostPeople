using LostPeople.Application.Common.Interfaces;
using LostPeople.Domain.Entities;
using LostPeople.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LostPeople.Infrastructure.Scraping;

public class HospitalSimuladoConnector : IDataSourceConnector
{
    private readonly ILogger<HospitalSimuladoConnector> _logger;
    private readonly LostPeopleDbContext _context;

    private static readonly string[] NombresHombres =
        { "Juan", "Carlos", "Pedro", "Jose", "Ramon", "Luis", "Miguel", "Franklin", "Rafael", "Manuel",
          "Victor", "Julio", "Antonio", "Fernando", "Alberto", "Enrique", "Eduardo", "Ricardo", "Jorge", "David" };
    private static readonly string[] NombresMujeres =
        { "Maria", "Ana", "Luisa", "Carmen", "Marta", "Rosa", "Juana", "Dulce", "Altagracia", "Milagros",
          "Yolanda", "Teresa", "Sandra", "Patricia", "Elizabeth", "Cristina", "Ruth", "Gisselle", "Katherine", "Laura" };
    private static readonly string[] Apellidos =
        { "Perez", "Garcia", "Martinez", "Rodriguez", "Lopez", "Hernandez", "Diaz", "Torres", "Ramirez",
          "Castillo", "Santos", "Reyes", "Cruz", "Jimenez", "Morales", "Vasquez", "Ortiz", "Nunez", "Almonte", "Pena" };
    private static readonly string[] ProvinciasRD =
        { "Distrito Nacional", "Santiago", "Santo Domingo", "La Vega", "Puerto Plata", "San Pedro de Macoris",
          "Duarte", "Barahona", "La Altagracia", "Espaillat", "San Cristobal", "La Romana" };
    private static readonly string[] HospitalesRD =
        { "Hospital Dr. Dario Contreras", "Hospital Dr. Luis E. Aybar", "Hospital Dr. Robert Reid Cabral",
          "Hospital Dr. Salvador B. Gautier", "Hospital Dr. Juan M. Taveras", "Hospital Dr. Francisco Moscoso Puello",
          "Hospital Dr. Arturo Grullon", "Hospital Dr. Antonio Musa", "Hospital Regional Dr. Antonio Yapor",
          "Hospital Dr. Toribio Bencosme" };
    private static readonly string[] SenasParticulares =
        { "tatuaje de cruz en brazo derecho", "cicatriz en ceja izquierda", "lunar grande en mejilla",
          "aretes de aro dorado", "tatuaje de nombre en brazo", "cicatriz de apendicitis",
          "sin senas particulares", "lentes de armazon negro", "barba espesa", "cabello pintado rubio" };
    private static readonly string[] Vestimentas =
        { "camiseta blanca y jeans azules", "uniforme escolar", "short azul y camisa a cuadros",
          "vestido floreado", "camisa de manga larga y pantalon negro", "franela deportiva y bermuda",
          "camisa de trabajo beige", "bata de hospital", "pijama azul claro", "ropa deportiva gris" };

    public HospitalSimuladoConnector(ILogger<HospitalSimuladoConnector> logger, LostPeopleDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public string SourceCode => "HOSPITAL_SIMULADO";
    public string SourceName => "Hospitales RD - Simulación de pacientes NN";

    public bool CanHandle(string sourceType) =>
        sourceType.Equals("SIMULATED", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("SIMULADO", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("HOSPITAL_SIMULADO", StringComparison.OrdinalIgnoreCase);

    public async Task<IngestionResult> FetchAsync(CancellationToken ct = default)
    {
        var startTime = DateTime.UtcNow;
        var rand = new Random(DateTime.UtcNow.Millisecond);
        var result = new IngestionResult();

        var provinciasDb = await _context.ZonasGeograficas
            .Where(z => z.Tipo == "Provincia")
            .ToListAsync(ct);

        var personas = new List<PersonaReportada>();
        for (int i = 0; i < 15; i++)
        {
            var esHombre = rand.Next(2) == 0;
            var nombres = esHombre ? NombresHombres : NombresMujeres;
            var nombre = nombres[rand.Next(nombres.Length)];
            var apellido1 = Apellidos[rand.Next(Apellidos.Length)];
            var apellido2 = Apellidos[rand.Next(Apellidos.Length)];
            var edad = rand.Next(60) + 2;
            var provincia = provinciasDb.Count > 0
                ? provinciasDb[rand.Next(provinciasDb.Count)].Nombre
                : ProvinciasRD[rand.Next(ProvinciasRD.Length)];
            var hospital = HospitalesRD[rand.Next(HospitalesRD.Length)];
            var vestimenta = Vestimentas[rand.Next(Vestimentas.Length)];
            var senas = SenasParticulares[rand.Next(SenasParticulares.Length)];

            var persona = new PersonaReportada
            {
                PrimerNombre = nombre,
                PrimerApellido = apellido1,
                SegundoApellido = apellido2,
                EdadAproximada = edad,
                Sexo = esHombre ? "M" : "F",
                UltimaUbicacionTexto = provincia,
                Vestimenta = vestimenta,
                SenasParticulares = senas,
                CodigoSeguimiento = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                DatosSinteticos = true,
                EstadoCasoId = 1,
                FechaCreacion = DateTime.UtcNow
            };
            personas.Add(persona);

            result.RecordsInserted++;
            result.RecordsExtracted++;

            _logger.LogDebug(
                "HospitalSimulado: paciente {Nombre} {Apellido}, {Edad} anos, {Provincia}, {Hospital}",
                nombre, $"{apellido1} {apellido2}", edad, provincia, hospital);
        }

        _context.PersonasReportadas.AddRange(personas);
        await _context.SaveChangesAsync(ct);

        result.Success = true;
        result.Duration = DateTime.UtcNow - startTime;
        _logger.LogInformation("HospitalSimulado: {Count} pacientes NN simulados generados", result.RecordsInserted);

        return result;
    }
}
