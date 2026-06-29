using LostPeople.Application.Common.Interfaces;

namespace LostPeople.Infrastructure.Scraping;

public class DataSourceConnectorFactory
{
    private readonly IEnumerable<IDataSourceConnector> _connectors;

    public DataSourceConnectorFactory(IEnumerable<IDataSourceConnector> connectors)
    {
        _connectors = connectors;
    }

    public IDataSourceConnector? GetConnector(string sourceType)
    {
        return _connectors.FirstOrDefault(c => c.CanHandle(sourceType));
    }

    public IDataSourceConnector? GetConnectorByCode(string code)
    {
        return _connectors.FirstOrDefault(c =>
            c.SourceCode.Equals(code, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<IDataSourceConnector> GetAllConnectors()
    {
        return _connectors;
    }
}
