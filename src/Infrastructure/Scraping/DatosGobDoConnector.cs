using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LostPeople.Application.Common.Interfaces;
using LostPeople.Domain.Entities;
using LostPeople.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LostPeople.Infrastructure.Scraping;

public class DatosGobDoConnector : IDataSourceConnector
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DatosGobDoConnector> _logger;
    private readonly LostPeopleDbContext _context;

    public string SourceCode => "DATOS_GOB_DO";
    public string SourceName => "Portal de Datos Abiertos (datos.gob.do) - Personas desaparecidas";

    private static readonly string[] UrlsParaProbar =
    {
        "https://datos.gob.do/api/3/action/package_list",
        "https://datos.gob.do/api/3/action/package_search?q=personas+desaparecidas",
        "https://datos.gob.do/api/3/action/package_search?q=desaparecido",
        "https://datos.gob.do/api/3/action/datastore_search?resource_id=personas-desaparecidas"
    };

    public DatosGobDoConnector(HttpClient httpClient, ILogger<DatosGobDoConnector> logger, LostPeopleDbContext context)
    {
        _httpClient = httpClient;
        _logger = logger;
        _context = context;
    }

    public bool CanHandle(string sourceType) =>
        sourceType.Equals("API", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("API_REST", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("DATOS_GOB_DO", StringComparison.OrdinalIgnoreCase);

    public async Task<IngestionResult> FetchAsync(CancellationToken ct = default)
    {
        var result = new IngestionResult();
        var startTime = DateTime.UtcNow;

        var fuente = await _context.FuentesDatos.FirstOrDefaultAsync(f => f.Codigo == SourceCode, ct);
        if (fuente == null)
        {
            result.Errors.Add(new IngestionError { Type = "CONFIG", Message = $"FuenteDatos '{SourceCode}' not found in database", IsFatal = true });
            result.Duration = DateTime.UtcNow - startTime;
            return result;
        }

        foreach (var url in UrlsParaProbar)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                _logger.LogInformation("DatosGobDo: consultando {Url}", url);
                var response = await _httpClient.GetAsync(url, ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("DatosGobDo: HTTP {Status} en {Url}", (int)response.StatusCode, url);
                    continue;
                }

                var json = await response.Content.ReadAsStringAsync(ct);
                result.RawResponsePreview = json.Length > 2000 ? json[..2000] : json;

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("success", out var success) && success.GetBoolean() &&
                    root.TryGetProperty("result", out var resultElement))
                {
                    var nuevosRegistros = new List<RegistroIngerido>();

                    if (resultElement.ValueKind == JsonValueKind.Array)
                    {
                        var items = resultElement.EnumerateArray().ToList();
                        result.RecordsExtracted = items.Count;

                        foreach (var item in items)
                        {
                            var name = item.GetString();
                            if (string.IsNullOrEmpty(name)) continue;

                            if (!name.Contains("desaparecido", StringComparison.OrdinalIgnoreCase) &&
                                !name.Contains("persona", StringComparison.OrdinalIgnoreCase) &&
                                !name.Contains("localizado", StringComparison.OrdinalIgnoreCase) &&
                                !name.Contains("buscado", StringComparison.OrdinalIgnoreCase))
                                continue;

                            var hash = ComputeHash(name);
                            var existe = await _context.RegistrosIngeridos.AnyAsync(r => r.HashContenido == hash && r.FuenteId == fuente.Id, ct);
                            if (existe)
                            {
                                result.RecordsDuplicated++;
                                continue;
                            }

                            nuevosRegistros.Add(new RegistroIngerido
                            {
                                FuenteId = fuente.Id,
                                IdentificadorExterno = name,
                                PrimerNombre = name,
                                UrlOrigen = url,
                                FechaIngesta = DateTime.UtcNow,
                                HashContenido = hash,
                                CoincidenciaProcesada = false
                            });
                        }
                    }
                    else if (resultElement.TryGetProperty("results", out var results))
                    {
                        result.RecordsExtracted = results.GetArrayLength();
                        foreach (var r in results.EnumerateArray())
                        {
                            var title = r.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "";
                            if (!title.Contains("desaparecido", StringComparison.OrdinalIgnoreCase)) continue;

                            var descripcion = r.TryGetProperty("notes", out var notes) ? notes.GetString() : null;
                            var idExterno = r.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;

                            var contentToHash = title + (descripcion ?? "") + (idExterno ?? "");
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
                                IdentificadorExterno = idExterno,
                                PrimerNombre = title,
                                DescripcionFisica = descripcion?.Length > 2000 ? descripcion[..2000] : descripcion,
                                UrlOrigen = url,
                                FechaIngesta = DateTime.UtcNow,
                                HashContenido = hash,
                                CoincidenciaProcesada = false
                            });
                        }
                    }

                    if (nuevosRegistros.Count > 0)
                    {
                        _context.RegistrosIngeridos.AddRange(nuevosRegistros);
                        await _context.SaveChangesAsync(ct);
                    }

                    result.RecordsInserted += nuevosRegistros.Count;
                    result.Success = true;
                    _logger.LogInformation("DatosGobDo: {Extracted} extraídos, {Inserted} nuevos insertados de {Url}",
                        result.RecordsExtracted, result.RecordsInserted, url);
                    break;
                }

                _logger.LogWarning("DatosGobDo: respuesta sin 'success' o 'result' en {Url}", url);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("DatosGobDo: timeout en {Url}", url);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "DatosGobDo: error HTTP en {Url}", url);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "DatosGobDo: error parseando JSON de {Url}", url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DatosGobDo: error inesperado en {Url}", url);
            }
        }

        if (result.RecordsExtracted == 0 && result.Errors.Count == 0)
        {
            result.Errors.Add(new IngestionError { Type = "NO_DATA", Message = "No se encontraron datos en ninguna URL", IsFatal = false });
            _logger.LogWarning("DatosGobDo: no se encontraron datos en ninguna URL");
        }

        result.Duration = DateTime.UtcNow - startTime;
        return result;
    }

    private static string ComputeHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
