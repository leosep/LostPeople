namespace LostPeople.Application.Common.Interfaces;

public interface IDataSourceConnector
{
    string SourceCode { get; }
    string SourceName { get; }
    Task<IngestionResult> FetchAsync(CancellationToken ct = default);
    bool CanHandle(string sourceType);
}

public class IngestionResult
{
    public bool Success { get; set; }
    public int RecordsExtracted { get; set; }
    public int RecordsInserted { get; set; }
    public int RecordsDuplicated { get; set; }
    public int RecordsInvalid { get; set; }
    public TimeSpan Duration { get; set; }
    public List<IngestionError> Errors { get; set; } = new();
    public string? RawResponsePreview { get; set; }
}

public class IngestionError
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Detail { get; set; }
    public bool IsFatal { get; set; }
}
