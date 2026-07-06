using LostPeople.Application.Common.Interfaces;
using LostPeople.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LostPeople.Application.PersonasReportadas.Commands;

public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateReportCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateReportCommand request, CancellationToken ct)
    {
        if (!request.AceptoTerminos || !request.AceptoConfidencialidad)
            throw new InvalidOperationException("Debe aceptar términos y confidencialidad");

        var codigo = GenerateCodigoSeguimiento();

        var persona = new PersonaReportada
        {
            PrimerNombre = request.PrimerNombre,
            SegundoNombre = request.SegundoNombre,
            PrimerApellido = request.PrimerApellido,
            SegundoApellido = request.SegundoApellido,
            Alias = request.Alias,
            FechaNacimiento = request.FechaNacimiento,
            FechaDesaparicion = request.FechaDesaparicion ?? DateTime.UtcNow,
            EdadAproximada = request.EdadAproximada,
            Sexo = request.Sexo,
            DescripcionFisica = request.DescripcionFisica,
            EstaturaCm = request.EstaturaCm,
            ColorPiel = request.ColorPiel,
            ColorOjos = request.ColorOjos,
            ColorCabello = request.ColorCabello,
            SenasParticulares = request.SenasParticulares,
            CondicionMedica = request.CondicionMedica,
            MedicamentosRequeridos = request.MedicamentosRequeridos,
            Vestimenta = request.Vestimenta,
            UltimaUbicacionTexto = request.UltimaUbicacionTexto,
            UltimaUbicacionLat = request.UltimaUbicacionLat,
            UltimaUbicacionLng = request.UltimaUbicacionLng,
            UltimaUbicacionZonaId = request.UltimaUbicacionZonaId,
            CodigoSeguimiento = codigo,
            EstadoCasoId = 1,
            DatosSinteticos = false,
            FechaCreacion = DateTime.UtcNow
        };

        _context.PersonasReportadas.Add(persona);
        await _context.SaveChangesAsync(ct);

        var usuarioAnonimo = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == "anonimo@lostpeople.do", ct);
        if (usuarioAnonimo == null)
        {
            usuarioAnonimo = new Usuario
            {
                NombreCompleto = "Anónimo",
                Email = "anonimo@lostpeople.do",
                PasswordHash = Guid.NewGuid().ToString(),
                RolId = 1,
                Activo = true,
                FechaCreacion = DateTime.UtcNow,
                AceptoTerminos = request.AceptoTerminos,
                AceptoConfidencialidad = request.AceptoConfidencialidad
            };
            _context.Usuarios.Add(usuarioAnonimo);
            await _context.SaveChangesAsync(ct);
        }

        var reporte = new Reporte
        {
            PersonaId = persona.Id,
            ReportanteUsuarioId = usuarioAnonimo.Id,
            RelacionConDesaparecido = request.RelacionConDesaparecido,
            TelefonoContacto = request.TelefonoContacto,
            EmailContacto = request.EmailContacto,
            CodigoVerificacion = GenerateCodigoVerificacion(),
            Verificado = false,
            FuenteReporte = "Web",
            FechaCreacion = DateTime.UtcNow
        };

        _context.Reportes.Add(reporte);
        await _context.SaveChangesAsync(ct);

        return persona.Id;
    }

    private static string GenerateCodigoSeguimiento()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var codigo = new char[5];
        var random = Random.Shared;
        for (int i = 0; i < 5; i++)
            codigo[i] = chars[random.Next(chars.Length)];
        return $"LP-{new string(codigo)}";
    }

    private static string GenerateCodigoVerificacion()
    {
        return Random.Shared.Next(100000, 999999).ToString();
    }
}
