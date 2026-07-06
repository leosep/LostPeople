using LostPeople.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LostPeople.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<PersonaReportada> PersonasReportadas { get; }
    DbSet<RegistroIngerido> RegistrosIngeridos { get; }
    DbSet<Coincidencia> Coincidencias { get; }
    DbSet<Reporte> Reportes { get; }
    DbSet<FuenteDatos> FuentesDatos { get; }
    DbSet<Usuario> Usuarios { get; }
    DbSet<Verificacion> Verificaciones { get; }
    DbSet<Archivo> Archivos { get; }
    DbSet<ZonaGeografica> ZonasGeograficas { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
