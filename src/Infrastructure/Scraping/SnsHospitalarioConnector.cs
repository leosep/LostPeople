using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LostPeople.Application.Common.Interfaces;
using LostPeople.Domain.Entities;
using LostPeople.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LostPeople.Infrastructure.Scraping;

public class SnsHospitalarioConnector : IDataSourceConnector
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SnsHospitalarioConnector> _logger;
    private readonly LostPeopleDbContext _context;

    public string SourceCode => "SNC_HOSPITALARIO";
    public string SourceName => "SNS - Pacientes NN en centros hospitalarios";

    public SnsHospitalarioConnector(HttpClient httpClient, ILogger<SnsHospitalarioConnector> logger, LostPeopleDbContext context)
    {
        _httpClient = httpClient;
        _logger = logger;
        _context = context;
    }

    public bool CanHandle(string sourceType) =>
        sourceType.Equals("API", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("API_REST", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("SNC_HOSPITALARIO", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("SNS", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("HOSPITAL", StringComparison.OrdinalIgnoreCase);

    public async Task<IngestionResult> FetchAsync(CancellationToken ct = default)
    {
        var result = new IngestionResult();
        var startTime = DateTime.UtcNow;

        var fuente = await _context.FuentesDatos.FirstOrDefaultAsync(f => f.Codigo == SourceCode, ct);
        if (fuente == null || !fuente.Activo)
        {
            result.Errors.Add(new IngestionError { Type = "CONFIG", Message = $"FuenteDatos '{SourceCode}' no encontrada o inactiva", IsFatal = false });
            result.Duration = DateTime.UtcNow - startTime;
            return result;
        }

        var centrosActivos = await _context.CentrosSalud
            .Where(c => c.Activo)
            .Include(c => c.Zona)
            .ToListAsync(ct);

        if (centrosActivos.Count == 0)
        {
            _logger.LogWarning("SnsHospitalario: no hay centros de salud activos configurados");
            result.Errors.Add(new IngestionError { Type = "NO_DATA", Message = "No hay centros de salud activos", IsFatal = false });
            result.Duration = DateTime.UtcNow - startTime;
            return result;
        }

        var nuevosRegistros = new List<RegistroIngerido>();

        foreach (var centro in centrosActivos)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                var url = string.IsNullOrEmpty(fuente.UrlBase)
                    ? $"https://api.sns.gob.do/api/pacientes-nn?centro={centro.Codigo}"
                    : $"{fuente.UrlBase}?centro={centro.Codigo}";

                _logger.LogInformation("SnsHospitalario: consultando centro {Centro} en {Url}", centro.Nombre, url);
                var response = await _httpClient.GetAsync(url, ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("SnsHospitalario: HTTP {Status} para {Centro}", (int)response.StatusCode, centro.Nombre);
                    continue;
                }

                var json = await response.Content.ReadAsStringAsync(ct);

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var pacientes = root.ValueKind == JsonValueKind.Array
                    ? root.EnumerateArray().ToList()
                    : root.TryGetProperty("pacientes", out var p) ? p.EnumerateArray().ToList()
                    : root.TryGetProperty("data", out var d) ? d.EnumerateArray().ToList()
                    : root.TryGetProperty("resultados", out var r) ? r.EnumerateArray().ToList()
                    : new List<JsonElement>();

                result.RecordsExtracted += pacientes.Count;

                foreach (var paciente in pacientes)
                {
                    var nombre = GetStringProperty(paciente, "nombre", "nombre_paciente", "paciente", "apodo");
                    var edad = paciente.TryGetProperty("edad", out var e) && e.ValueKind == JsonValueKind.Number ? e.GetInt32() : (int?)null;
                    var sexo = GetStringProperty(paciente, "sexo", "genero");
                    var descripcion = GetStringProperty(paciente, "descripcion", "observaciones", "notas", "senas");
                    var ubicacion = centro.Zona?.Nombre ?? centro.Direccion;
                    var fechaIngresoStr = GetStringProperty(paciente, "fecha_ingreso", "fecha", "fecha_registro");
                    var estado = GetStringProperty(paciente, "estado", "condicion", "estado_paciente");

                    var contentToHash = $"{centro.Codigo}|{nombre}|{edad}|{sexo}|{descripcion}";
                    var hash = ComputeHash(contentToHash);

                    var existe = await _context.RegistrosIngeridos.AnyAsync(r => r.HashContenido == hash && r.FuenteId == fuente.Id, ct);
                    if (existe)
                    {
                        result.RecordsDuplicated++;
                        continue;
                    }

                    DateTime? fechaRegistro = null;
                    if (!string.IsNullOrEmpty(fechaIngresoStr) && DateTime.TryParse(fechaIngresoStr, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedFecha))
                        fechaRegistro = parsedFecha;

                    nuevosRegistros.Add(new RegistroIngerido
                    {
                        FuenteId = fuente.Id,
                        PrimerNombre = nombre,
                        Sexo = sexo,
                        EdadAproximada = edad,
                        DescripcionFisica = descripcion?.Length > 2000 ? descripcion[..2000] : descripcion,
                        UbicacionTexto = ubicacion,
                        InstitucionOrigen = centro.Nombre,
                        UrlOrigen = url,
                        EstadoPaciente = estado,
                        FechaRegistroFuente = fechaRegistro,
                        FechaIngesta = DateTime.UtcNow,
                        HashContenido = hash,
                        CoincidenciaProcesada = false
                    });
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("SnsHospitalario: timeout en {Centro}", centro.Nombre);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "SnsHospitalario: error HTTP en {Centro}", centro.Nombre);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "SnsHospitalario: error parseando JSON en {Centro}", centro.Nombre);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SnsHospitalario: error inesperado en {Centro}", centro.Nombre);
            }
        }

        if (nuevosRegistros.Count > 0)
        {
            _context.RegistrosIngeridos.AddRange(nuevosRegistros);
            await _context.SaveChangesAsync(ct);
        }

        result.RecordsInserted = nuevosRegistros.Count;
        result.Success = result.RecordsExtracted > 0 || nuevosRegistros.Count > 0;

        _logger.LogInformation("SnsHospitalario: {Extracted} extraídos, {Inserted} nuevos de {Centros} centros",
            result.RecordsExtracted, result.RecordsInserted, centrosActivos.Count);

        result.Duration = DateTime.UtcNow - startTime;
        return result;
    }

    private static string? GetStringProperty(JsonElement element, params string[] posiblesNombres)
    {
        foreach (var name in posiblesNombres)
        {
            if (element.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String)
            {
                var val = prop.GetString();
                if (!string.IsNullOrWhiteSpace(val)) return val.Trim();
            }
        }
        return null;
    }

    private static string ComputeHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
