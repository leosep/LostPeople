using LostPeople.Domain.Interfaces;

namespace LostPeople.Domain.Entities;

public class FuenteDatos : IEntity
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? MetodoAcceso { get; set; }
    public string? RiesgoLegal { get; set; }
    public string? UrlBase { get; set; }
    public string? FormatoDatos { get; set; }
    public int IntervaloMinutos { get; set; } = 30;
    public string? SelectorHtml { get; set; }
    public string? ConfiguracionJson { get; set; }
    public bool Activo { get; set; } = true;
    public string? EstadoSalud { get; set; }
    public DateTime? UltimaEjecucionOk { get; set; }
    public DateTime? UltimoError { get; set; }
    public int FallosConsecutivos { get; set; }
    public int TotalEjecuciones { get; set; }
    public int TotalRegistrosObtenidos { get; set; }
    public string? NotasLegales { get; set; }

    public ICollection<RegistroIngerido> RegistrosIngeridos { get; set; } = new List<RegistroIngerido>();
}
