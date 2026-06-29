using System.Text.Json;
using LostPeople.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace LostPeople.Infrastructure.Scraping;

public class DatosGobDoConnector : IDataSourceConnector
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DatosGobDoConnector> _logger;

    public string SourceCode => "DATOS_GOB_DO";
    public string SourceName => "Portal de Datos Abiertos (datos.gob.do) - Personas desaparecidas";

    private static readonly string[] UrlsParaProbar =
    {
        "https://datos.gob.do/api/3/action/package_list",
        "https://datos.gob.do/api/3/action/package_search?q=personas+desaparecidas",
        "https://datos.gob.do/api/3/action/package_search?q=desaparecido",
        "https://datos.gob.do/api/3/action/datastore_search?resource_id=personas-desaparecidas"
    };

    public DatosGobDoConnector(HttpClient httpClient, ILogger<DatosGobDoConnector> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public bool CanHandle(string sourceType) =>
        sourceType.Equals("API", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("API_REST", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("DATOS_GOB_DO", StringComparison.OrdinalIgnoreCase);

    public async Task<IngestionResult> FetchAsync(CancellationToken ct = default)
    {
        var result = new IngestionResult();
        var startTime = DateTime.UtcNow;

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
                    if (resultElement.ValueKind == JsonValueKind.Array)
                    {
                        var items = resultElement.EnumerateArray().ToList();
                        result.RecordsExtracted = items.Count;

                        foreach (var item in items)
                        {
                            var name = item.GetString();
                            if (!string.IsNullOrEmpty(name) &&
                                (name.Contains("desaparecido", StringComparison.OrdinalIgnoreCase) ||
                                 name.Contains("persona", StringComparison.OrdinalIgnoreCase) ||
                                 name.Contains("localizado", StringComparison.OrdinalIgnoreCase) ||
                                 name.Contains("buscado", StringComparison.OrdinalIgnoreCase)))
                            {
                                result.RecordsInserted++;
                                _logger.LogInformation("DatosGobDo: paquete relevante encontrado: {Name}", name);
                            }
                        }
                    }
                    else if (resultElement.TryGetProperty("results", out var results))
                    {
                        result.RecordsExtracted = results.GetArrayLength();
                        foreach (var r in results.EnumerateArray())
                        {
                            if (r.TryGetProperty("title", out var title))
                            {
                                var t = title.GetString() ?? "";
                                if (t.Contains("desaparecido", StringComparison.OrdinalIgnoreCase))
                                    result.RecordsInserted++;
                            }
                        }
                    }

                    result.Success = true;
                    _logger.LogInformation("DatosGobDo: {Extracted} extraídos, {Inserted} insertados de {Url}",
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
            result.Errors.Add(new IngestionError
            {
                Type = "NO_DATA",
                Message = "No se encontraron datos en ninguna URL",
                IsFatal = false
            });
            _logger.LogWarning("DatosGobDo: no se encontraron datos en ninguna URL");
        }

        result.Duration = DateTime.UtcNow - startTime;
        return result;
    }
}
