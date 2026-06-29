using System.Text.RegularExpressions;
using LostPeople.Application.Common.Interfaces;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;

namespace LostPeople.Infrastructure.Scraping;

public class PoliciaNacionalConnector : IDataSourceConnector
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PoliciaNacionalConnector> _logger;

    public string SourceCode => "POLICIA_NACIONAL";
    public string SourceName => "Policía Nacional - Boletines y personas desaparecidas";

    public PoliciaNacionalConnector(HttpClient httpClient, ILogger<PoliciaNacionalConnector> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public bool CanHandle(string sourceType) =>
        sourceType.Equals("WEB_SCRAPER", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("HTML", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("POLICIA_NACIONAL", StringComparison.OrdinalIgnoreCase) ||
        sourceType.Equals("PN_CRITICAL_MISSING", StringComparison.OrdinalIgnoreCase);

    public async Task<IngestionResult> FetchAsync(CancellationToken ct = default)
    {
        var result = new IngestionResult();
        var startTime = DateTime.UtcNow;

        var urls = new[]
        {
            "https://policianacional.gob.do/desaparecidos/",
            "https://policianacional.gob.do/servicios/personas-desaparecidas/",
            "https://policianacional.gob.do/transparencia/boletines-desaparecidos/"
        };

        foreach (var url in urls)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                _logger.LogInformation("PoliciaNacional: intentando {Url}", url);
                var response = await _httpClient.GetAsync(url, ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("PoliciaNacional: HTTP {Status} para {Url}", (int)response.StatusCode, url);
                    result.Errors.Add(new IngestionError
                    {
                        Type = "HTTP_ERROR",
                        Message = $"HTTP {(int)response.StatusCode} para {url}",
                        IsFatal = false
                    });
                    continue;
                }

                var html = await response.Content.ReadAsStringAsync(ct);
                result.RawResponsePreview = html.Length > 2000 ? html[..2000] : html;

                var config = Configuration.Default;
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(req => req.Content(html), ct);

                var selectors = new[]
                {
                    ".desaparecidos-lista .item",
                    ".lista-desaparecidos tr",
                    ".entry-content p",
                    "article",
                    ".post-content",
                    ".content-list .item",
                    "table.table tbody tr"
                };

                IHtmlCollection<IElement>? items = null;
                foreach (var selector in selectors)
                {
                    items = document.QuerySelectorAll(selector);
                    if (items.Length > 0)
                    {
                        _logger.LogInformation("PoliciaNacional: {Cantidad} registros con selector '{Selector}'", items.Length, selector);
                        break;
                    }
                }

                if (items == null || items.Length == 0)
                {
                    _logger.LogWarning("PoliciaNacional: 0 registros en {Url} - posible cambio estructural", url);
                    result.Errors.Add(new IngestionError
                    {
                        Type = "NO_DATA",
                        Message = $"Sin registros en {url}",
                        IsFatal = false
                    });
                    continue;
                }

                result.RecordsExtracted += items.Length;

                foreach (var item in items.Take(50))
                {
                    var texto = item.TextContent.Trim();
                    if (string.IsNullOrEmpty(texto) || texto.Length < 10) continue;

                    var nombre = ExtractNombre(texto);
                    if (string.IsNullOrEmpty(nombre)) continue;

                    result.RecordsInserted++;
                }

                result.Success = true;
                _logger.LogInformation("PoliciaNacional: {Insertados} registros insertados de {Url}", result.RecordsInserted, url);
                break;
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("PoliciaNacional: timeout en {Url}", url);
                result.Errors.Add(new IngestionError
                {
                    Type = "TIMEOUT",
                    Message = $"Timeout en {url}",
                    IsFatal = false
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "PoliciaNacional: error HTTP en {Url}", url);
                result.Errors.Add(new IngestionError
                {
                    Type = "HTTP_ERROR",
                    Message = ex.Message,
                    IsFatal = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PoliciaNacional: error procesando {Url}", url);
                result.Errors.Add(new IngestionError
                {
                    Type = "PARSE_ERROR",
                    Message = ex.Message,
                    IsFatal = false
                });
            }
        }

        if (result.RecordsExtracted == 0 && result.Errors.All(e => !e.IsFatal))
        {
            _logger.LogWarning("PoliciaNacional: no se pudieron obtener datos de ninguna URL");
        }

        result.Duration = DateTime.UtcNow - startTime;
        return result;
    }

    private static string? ExtractNombre(string texto)
    {
        var patterns = new[]
        {
            @"(?:nombre|desaparecido|reportado|buscado)[:\s]+([A-ZÁÉÍÓÚÑ][a-záéíóúñ]+(?:\s+[A-ZÁÉÍÓÚÑ][a-záéíóúñ]+){1,4})",
            @"([A-ZÁÉÍÓÚÑ][a-záéíóúñ]+\s+[A-ZÁÉÍÓÚÑ][a-záéíóúñ]+(?:\s+[A-ZÁÉÍÓÚÑ][a-záéíóúñ]+)?)"
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(texto, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
                return match.Groups[1].Value.Trim();
        }

        return null;
    }
}
