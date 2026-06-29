using Microsoft.EntityFrameworkCore;
using LostPeople.Domain.Entities;

namespace LostPeople.Infrastructure.Persistence;

public class LostPeopleDbContext : DbContext
{
    public LostPeopleDbContext(DbContextOptions<LostPeopleDbContext> options) : base(options) { }

    public DbSet<PersonaReportada> PersonasReportadas => Set<PersonaReportada>();
    public DbSet<Reporte> Reportes => Set<Reporte>();
    public DbSet<EstadoCaso> EstadosCaso => Set<EstadoCaso>();
    public DbSet<ZonaGeografica> ZonasGeograficas => Set<ZonaGeografica>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<FuenteDatos> FuentesDatos => Set<FuenteDatos>();
    public DbSet<RegistroIngerido> RegistrosIngeridos => Set<RegistroIngerido>();
    public DbSet<Coincidencia> Coincidencias => Set<Coincidencia>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
    public DbSet<Auditoria> Auditorias => Set<Auditoria>();
    public DbSet<Archivo> Archivos => Set<Archivo>();
    public DbSet<CentroSalud> CentrosSalud => Set<CentroSalud>();
    public DbSet<Verificacion> Verificaciones => Set<Verificacion>();
    public DbSet<SesionUsuario> SesionesUsuario => Set<SesionUsuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PersonaReportada>(e =>
        {
            e.ToTable("PersonasReportadas");
            e.HasKey(x => x.Id);
            e.Property(x => x.PrimerNombre).HasMaxLength(100).IsRequired();
            e.Property(x => x.SegundoNombre).HasMaxLength(100);
            e.Property(x => x.PrimerApellido).HasMaxLength(100).IsRequired();
            e.Property(x => x.SegundoApellido).HasMaxLength(100);
            e.Property(x => x.Alias).HasMaxLength(100);
            e.Property(x => x.Sexo).HasMaxLength(20);
            e.Property(x => x.TipoDocumento).HasMaxLength(30);
            e.Property(x => x.NumeroDocumento).HasMaxLength(30);
            e.Property(x => x.Nacionalidad).HasMaxLength(50);
            e.Property(x => x.ColorPiel).HasMaxLength(30);
            e.Property(x => x.ColorOjos).HasMaxLength(30);
            e.Property(x => x.ColorCabello).HasMaxLength(30);
            e.Property(x => x.CondicionMedica).HasMaxLength(500);
            e.Property(x => x.MedicamentosRequeridos).HasMaxLength(500);
            e.Property(x => x.Vestimenta).HasMaxLength(500);
            e.Property(x => x.TipoAlerta).HasMaxLength(20);
            e.Property(x => x.CodigoSeguimiento).HasMaxLength(20).IsRequired();
            e.Property(x => x.UltimaUbicacionTexto).HasMaxLength(500);
            e.Property(x => x.DescripcionFisica).HasMaxLength(2000);
            e.Property(x => x.SenasParticulares).HasMaxLength(2000);
            e.Property(x => x.EstaturaCm).HasPrecision(5, 1);
            e.Property(x => x.PesoKg).HasPrecision(5, 1);
            e.Property(x => x.UltimaUbicacionLat).HasPrecision(10, 7);
            e.Property(x => x.UltimaUbicacionLng).HasPrecision(10, 7);
            e.HasIndex(x => x.CodigoSeguimiento).IsUnique();
            e.HasIndex(x => new { x.PrimerNombre, x.SegundoNombre, x.PrimerApellido, x.SegundoApellido });
            e.HasIndex(x => x.EstadoCasoId);
            e.HasIndex(x => x.FechaDesaparicion);
            e.HasIndex(x => x.DatosSinteticos).HasFilter("[DatosSinteticos] = 0");
            e.HasOne(x => x.EstadoCaso).WithMany(x => x.Personas).HasForeignKey(x => x.EstadoCasoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.UltimaUbicacionZona).WithMany(x => x.PersonasReportadas).HasForeignKey(x => x.UltimaUbicacionZonaId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Reporte>(e =>
        {
            e.ToTable("Reportes");
            e.HasKey(x => x.Id);
            e.Property(x => x.RelacionConDesaparecido).HasMaxLength(100);
            e.Property(x => x.TelefonoContacto).HasMaxLength(20);
            e.Property(x => x.EmailContacto).HasMaxLength(200);
            e.Property(x => x.CodigoVerificacion).HasMaxLength(10);
            e.Property(x => x.Notas).HasMaxLength(2000);
            e.Property(x => x.FuenteReporte).HasMaxLength(50);
            e.Property(x => x.IpOrigen).HasMaxLength(50);
            e.HasOne(x => x.Persona).WithMany(x => x.Reportes).HasForeignKey(x => x.PersonaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Reportante).WithMany(x => x.Reportes).HasForeignKey(x => x.ReportanteUsuarioId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EstadoCaso>(e =>
        {
            e.ToTable("EstadosCaso");
            e.HasKey(x => x.Id);
            e.Property(x => x.Codigo).HasMaxLength(30).IsRequired();
            e.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
            e.Property(x => x.ColorHex).HasMaxLength(10);
            e.HasIndex(x => x.Codigo).IsUnique();
        });

        modelBuilder.Entity<ZonaGeografica>(e =>
        {
            e.ToTable("ZonasGeograficas");
            e.HasKey(x => x.Id);
            e.Property(x => x.Codigo).HasMaxLength(20).IsRequired();
            e.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
            e.Property(x => x.Tipo).HasMaxLength(20).IsRequired();
            e.Property(x => x.LatitudCentroide).HasPrecision(10, 7);
            e.Property(x => x.LongitudCentroide).HasPrecision(10, 7);
            e.HasOne(x => x.ZonaPadre).WithMany(x => x.Hijos).HasForeignKey(x => x.ZonaPadreId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.Tipo);
        });

        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios");
            e.HasKey(x => x.Id);
            e.Property(x => x.NombreCompleto).HasMaxLength(200).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.Telefono).HasMaxLength(20);
            e.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            e.Property(x => x.Cedula).HasMaxLength(20);
            e.Property(x => x.Institucion).HasMaxLength(200);
            e.Property(x => x.Cargo).HasMaxLength(200);
            e.Property(x => x.TokenVerificacion).HasMaxLength(100);
            e.HasIndex(x => x.Email).IsUnique();
            e.HasOne(x => x.Rol).WithMany(x => x.Usuarios).HasForeignKey(x => x.RolId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.ZonaAsignada).WithMany(x => x.UsuariosAsignados).HasForeignKey(x => x.ZonaAsignadaId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Rol>(e =>
        {
            e.ToTable("Roles");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nombre).HasMaxLength(50).IsRequired();
            e.Property(x => x.Descripcion).HasMaxLength(200);
            e.HasIndex(x => x.Nombre).IsUnique();
        });

        modelBuilder.Entity<FuenteDatos>(e =>
        {
            e.ToTable("FuentesDatos");
            e.HasKey(x => x.Id);
            e.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
            e.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
            e.Property(x => x.Tipo).HasMaxLength(30).IsRequired();
            e.Property(x => x.UrlBase).HasMaxLength(500);
            e.Property(x => x.SelectorHtml).HasMaxLength(500);
            e.Property(x => x.EstadoSalud).HasMaxLength(50);
            e.Property(x => x.NotasLegales).HasMaxLength(2000);
            e.HasIndex(x => x.Codigo).IsUnique();
        });

        modelBuilder.Entity<RegistroIngerido>(e =>
        {
            e.ToTable("RegistrosIngeridos");
            e.HasKey(x => x.Id);
            e.Property(x => x.PrimerNombre).HasMaxLength(100);
            e.Property(x => x.SegundoNombre).HasMaxLength(100);
            e.Property(x => x.PrimerApellido).HasMaxLength(100);
            e.Property(x => x.SegundoApellido).HasMaxLength(100);
            e.Property(x => x.Sexo).HasMaxLength(20);
            e.Property(x => x.DescripcionFisica).HasMaxLength(2000);
            e.Property(x => x.UbicacionTexto).HasMaxLength(500);
            e.Property(x => x.TelefonoContacto).HasMaxLength(20);
            e.Property(x => x.InstitucionOrigen).HasMaxLength(200);
            e.Property(x => x.UrlOrigen).HasMaxLength(1000);
            e.Property(x => x.EstadoPaciente).HasMaxLength(50);
            e.Property(x => x.HashContenido).HasMaxLength(64);
            e.Property(x => x.Latitud).HasPrecision(10, 7);
            e.Property(x => x.Longitud).HasPrecision(10, 7);
            e.HasIndex(x => new { x.PrimerNombre, x.PrimerApellido });
            e.HasIndex(x => x.HashContenido).IsUnique().HasFilter("[HashContenido] IS NOT NULL");
            e.HasIndex(x => x.CoincidenciaProcesada).HasFilter("[CoincidenciaProcesada] = 0");
            e.HasOne(x => x.Fuente).WithMany(x => x.RegistrosIngeridos).HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.FuenteId, x.FechaRegistroFuente });
        });

        modelBuilder.Entity<Coincidencia>(e =>
        {
            e.ToTable("Coincidencias");
            e.HasKey(x => x.Id);
            e.Property(x => x.AlgoritmoUsado).HasMaxLength(50);
            e.Property(x => x.ResultadoRevision).HasMaxLength(30);
            e.Property(x => x.NotasRevision).HasMaxLength(2000);
            e.Property(x => x.ScoreGeneral).HasPrecision(5, 2);
            e.Property(x => x.ScoreNombre).HasPrecision(5, 2);
            e.Property(x => x.ScoreEdad).HasPrecision(5, 2);
            e.Property(x => x.ScoreSexo).HasPrecision(5, 2);
            e.Property(x => x.ScoreUbicacion).HasPrecision(5, 2);
            e.Property(x => x.ScoreDescripcion).HasPrecision(5, 2);
            e.HasOne(x => x.PersonaReportada).WithMany(x => x.Coincidencias).HasForeignKey(x => x.ReportePersonaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.RegistroIngerido).WithMany(x => x.Coincidencias).HasForeignKey(x => x.RegistroIngeridoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Revisor).WithMany().HasForeignKey(x => x.RevisorUsuarioId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => x.Revisada).HasFilter("[Revisada] = 0");
            e.HasIndex(x => x.ScoreGeneral);
            e.HasIndex(x => new { x.Revisada, x.ScoreGeneral }).HasFilter("[Revisada] = 0");
        });

        modelBuilder.Entity<Notificacion>(e =>
        {
            e.ToTable("Notificaciones");
            e.HasKey(x => x.Id);
            e.Property(x => x.Tipo).HasMaxLength(50).IsRequired();
            e.Property(x => x.Titulo).HasMaxLength(200);
            e.Property(x => x.Mensaje).HasMaxLength(2000);
            e.Property(x => x.CanalEnvio).HasMaxLength(20);
            e.HasOne(x => x.Usuario).WithMany(x => x.Notificaciones).HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.PersonaReportada).WithMany(x => x.Notificaciones).HasForeignKey(x => x.ReportePersonaId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => new { x.UsuarioId, x.Leida, x.FechaCreacion });
        });

        modelBuilder.Entity<Auditoria>(e =>
        {
            e.ToTable("Auditorias");
            e.HasKey(x => x.Id);
            e.Property(x => x.Entidad).HasMaxLength(100);
            e.Property(x => x.TipoOperacion).HasMaxLength(50);
            e.Property(x => x.IpOrigen).HasMaxLength(50);
            e.Property(x => x.UserAgent).HasMaxLength(500);
            e.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => x.Fecha);
        });

        modelBuilder.Entity<Archivo>(e =>
        {
            e.ToTable("Archivos");
            e.HasKey(x => x.Id);
            e.Property(x => x.NombreOriginal).HasMaxLength(500).IsRequired();
            e.Property(x => x.RutaArchivo).HasMaxLength(1000).IsRequired();
            e.Property(x => x.TipoContenido).HasMaxLength(100);
            e.Property(x => x.Categoria).HasMaxLength(50);
            e.HasOne(x => x.Persona).WithMany(x => x.Archivos).HasForeignKey(x => x.PersonaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Reporte).WithMany(x => x.Archivos).HasForeignKey(x => x.ReporteId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.SubidoPorUsuario).WithMany(x => x.ArchivosSubidos).HasForeignKey(x => x.SubidoPorUsuarioId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CentroSalud>(e =>
        {
            e.ToTable("CentrosSalud");
            e.HasKey(x => x.Id);
            e.Property(x => x.Codigo).HasMaxLength(20).IsRequired();
            e.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
            e.Property(x => x.Tipo).HasMaxLength(50);
            e.Property(x => x.Direccion).HasMaxLength(500);
            e.Property(x => x.Telefono).HasMaxLength(20);
            e.Property(x => x.ContactoEmail).HasMaxLength(200);
            e.HasOne(x => x.Zona).WithMany(x => x.CentrosSalud).HasForeignKey(x => x.ZonaId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Verificacion>(e =>
        {
            e.ToTable("Verificaciones");
            e.HasKey(x => x.Id);
            e.Property(x => x.TipoVerificacion).HasMaxLength(50);
            e.Property(x => x.Resultado).HasMaxLength(30);
            e.Property(x => x.MetodoContacto).HasMaxLength(50);
            e.Property(x => x.DetalleContacto).HasMaxLength(500);
            e.Property(x => x.Notas).HasMaxLength(2000);
            e.HasOne(x => x.Coincidencia).WithMany(x => x.Verificaciones).HasForeignKey(x => x.CoincidenciaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Verificador).WithMany(x => x.Verificaciones).HasForeignKey(x => x.VerificadorUsuarioId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SesionUsuario>(e =>
        {
            e.ToTable("SesionesUsuario");
            e.HasKey(x => x.Id);
            e.Property(x => x.Token).HasMaxLength(500).IsRequired();
            e.Property(x => x.RefreshToken).HasMaxLength(500);
            e.Property(x => x.IpOrigen).HasMaxLength(50);
            e.Property(x => x.UserAgent).HasMaxLength(500);
            e.HasOne(x => x.Usuario).WithMany(x => x.Sesiones).HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
