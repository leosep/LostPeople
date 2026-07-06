using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LostPeople.Application.Common.Interfaces;
using LostPeople.Domain.Entities;
using LostPeople.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LostPeople.Infrastructure.Scraping;

public class Emergencias911Connector : IDataSourceConnector
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<Emergencias911Connector> _logger;
    private readonly LostPeopleDbContext _context;

    public string SourceCode => "911_EMERGENCIAS";
    public string SourceName => "Sistema 9-1-1 - Reportes de emergencia";

    public Emergencias911Connector(HttpClient httpClient, ILogger<Emergencias911Connector> logger, LostPeopleDbContext context)
    {
        _httpClient = httpClient;
        _logger = logger;
        _context = context;
    }

    public bool CanHandle(string sourceType) =>
        sourceType.Equals("API", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("911_EMERGENCIAS", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("EMERGENCIAS", StringComparison.OrdinalIgnoreCase);

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

        var urlBase = fuente.UrlBase ?? "https://api.911.gob.do/v1/reportes";

        try
        {
            _logger.LogInformation("Emergencias911: consultando {Url}", urlBase);
            var response = await _httpClient.GetAsync(urlBase, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Emergencias911: HTTP {Status}", (int)response.StatusCode);
                result.Errors.Add(new IngestionError { Type = "HTTP_ERROR", Message = $"HTTP {(int)response.StatusCode}", IsFatal = false });
                result.Duration = DateTime.UtcNow - startTime;
                return result;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            result.RawResponsePreview = json.Length > 2000 ? json[..2000] : json;

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var datos = root.ValueKind == JsonValueKind.Array
                ? root.EnumerateArray().ToList()
                : root.TryGetProperty("data", out var d) ? d.EnumerateArray().ToList()
                : root.TryGetProperty("resultados", out var r) ? r.EnumerateArray().ToList()
                : new List<JsonElement>();

            result.RecordsExtracted = datos.Count;
            var nuevosRegistros = new List<RegistroIngerido>();

            foreach (var item in datos.Take(200))
            {
                var nombre = GetStringProperty(item, "nombre", "nombre_persona", "paciente", "involucrado");
                if (string.IsNullOrEmpty(nombre)) nombre = GetStringProperty(item, "descripcion", "detalle", "reporte");

                var identificador = GetStringProperty(item, "id", "codigo", "folio", "expediente");
                var descripcion = GetStringProperty(item, "descripcion", "detalle", "observaciones", "relato");
                var ubicacion = GetStringProperty(item, "ubicacion", "direccion", "lugar", "zona");
                var telefono = GetStringProperty(item, "telefono", "contacto", "celular");
                var fechaStr = GetStringProperty(item, "fecha", "fecha_reporte", "fecha_evento", "created_at");

                var contentToHash = $"{identificador}|{nombre}|{descripcion}|{ubicacion}";
                var hash = ComputeHash(contentToHash);

                var existe = await _context.RegistrosIngeridos.AnyAsync(r => r.HashContenido == hash && r.FuenteId == fuente.Id, ct);
                if (existe)
                {
                    result.RecordsDuplicated++;
                    continue;
                }

                nuevosRegistros.Add(new RegistroIngerido
                {
                    FuenteId = fuente.Id,
                    IdentificadorExterno = identificador,
                    PrimerNombre = nombre,
                    DescripcionFisica = descripcion?.Length > 2000 ? descripcion[..2000] : descripcion,
                    UbicacionTexto = ubicacion,
                    TelefonoContacto = telefono,
                    InstitucionOrigen = "Sistema 9-1-1",
                    UrlOrigen = urlBase,
                    FechaIngesta = DateTime.UtcNow,
                    HashContenido = hash,
                    CoincidenciaProcesada = false
                });
            }

            if (nuevosRegistros.Count > 0)
            {
                _context.RegistrosIngeridos.AddRange(nuevosRegistros);
                await _context.SaveChangesAsync(ct);
            }

            result.RecordsInserted = nuevosRegistros.Count;
            result.Success = true;
            _logger.LogInformation("Emergencias911: {Extracted} extraídos, {Inserted} nuevos", result.RecordsExtracted, result.RecordsInserted);
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Emergencias911: timeout");
            result.Errors.Add(new IngestionError { Type = "TIMEOUT", Message = "Timeout en 9-1-1 API", IsFatal = false });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Emergencias911: error HTTP");
            result.Errors.Add(new IngestionError { Type = "HTTP_ERROR", Message = ex.Message, IsFatal = false });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Emergencias911: error parseando JSON");
            result.Errors.Add(new IngestionError { Type = "PARSE_ERROR", Message = ex.Message, IsFatal = false });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Emergencias911: error inesperado");
            result.Errors.Add(new IngestionError { Type = "UNEXPECTED", Message = ex.Message, IsFatal = false });
        }

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
