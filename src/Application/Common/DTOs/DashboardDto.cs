namespace LostPeople.Application.Common.DTOs;

public class DashboardDto
{
    public int CasosActivos { get; set; }
    public int CasosResueltos { get; set; }
    public int CoincidenciasPendientes { get; set; }
    public int FuentesActivas { get; set; }
    public int FuentesCaidas { get; set; }
    public double TiempoPromedioResolucionHoras { get; set; }
    public List<FuenteEstadoDto> Fuentes { get; set; } = new();
    public List<CasoRecienteDto> CasosRecientes { get; set; } = new();
}

public class FuenteEstadoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? UltimaEjecucion { get; set; }
    public int TotalRegistros { get; set; }
}

public class CasoRecienteDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaReporte { get; set; }
}
