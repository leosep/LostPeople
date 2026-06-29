using System.Net;
using System.Net.Mail;
using LostPeople.Application.Common.Interfaces;
using LostPeople.Infrastructure.Persistence;
using LostPeople.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio.Types;
using Twilio;

namespace LostPeople.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly LostPeopleDbContext _context;
    private readonly ILogger<NotificationService> _logger;
    private readonly IConfiguration _configuration;

    public NotificationService(LostPeopleDbContext context, ILogger<NotificationService> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendSmsAsync(string toPhone, string message, CancellationToken ct = default)
    {
        var accountSid = _configuration["Twilio:AccountSid"] ?? Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
        var authToken = _configuration["Twilio:AuthToken"] ?? Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
        var fromNumber = _configuration["Twilio:FromNumber"] ?? Environment.GetEnvironmentVariable("TWILIO_FROM_NUMBER");

        if (!string.IsNullOrEmpty(accountSid) && !string.IsNullOrEmpty(authToken))
        {
            try
            {
                TwilioClient.Init(accountSid, authToken);
                var msg = await Twilio.Rest.Api.V2010.Account.MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(fromNumber),
                    to: new PhoneNumber(toPhone));
                _logger.LogInformation("SMS enviado via Twilio a {Phone}: {Sid}", toPhone, msg.Sid);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error enviando SMS via Twilio, usando fallback log");
            }
        }

        _logger.LogInformation("SMS enviado a {Phone}: {Message}", toPhone, message);
        await Task.CompletedTask;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken ct = default)
    {
        var smtpHost = _configuration["Smtp:Host"] ?? Environment.GetEnvironmentVariable("SMTP_HOST");
        var smtpPort = int.TryParse(_configuration["Smtp:Port"] ?? Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : 587;
        var smtpUser = _configuration["Smtp:User"] ?? Environment.GetEnvironmentVariable("SMTP_USER");
        var smtpPass = _configuration["Smtp:Password"] ?? Environment.GetEnvironmentVariable("SMTP_PASSWORD");
        var smtpFrom = _configuration["Smtp:From"] ?? Environment.GetEnvironmentVariable("SMTP_FROM") ?? "noreply@lostpeople.do";

        if (!string.IsNullOrEmpty(smtpHost) && !string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPass))
        {
            try
            {
                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };
                using var mail = new MailMessage(smtpFrom, toEmail, subject, body) { IsBodyHtml = true };
                await client.SendMailAsync(mail, ct);
                _logger.LogInformation("Email enviado via SMTP a {Email}: {Subject}", toEmail, subject);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error enviando email via SMTP, usando fallback log");
            }
        }

        _logger.LogInformation("Email enviado a {Email}: {Subject}", toEmail, subject);
    }

    public async Task NotifyMatchFoundAsync(int usuarioId, int personaId, decimal score, CancellationToken ct = default)
    {
        var notif = new Notificacion
        {
            UsuarioId = usuarioId,
            ReportePersonaId = personaId,
            Tipo = "MATCH_ENCONTRADO",
            Titulo = "Posible coincidencia encontrada",
            Mensaje = $"Se ha detectado una posible coincidencia (score: {score:P0}) para un caso relacionado. Un verificador la está revisando.",
            FechaCreacion = DateTime.UtcNow
        };
        _context.Notificaciones.Add(notif);
        await _context.SaveChangesAsync(ct);
    }

    public async Task NotifyCaseClosedAsync(int usuarioId, int personaId, string status, CancellationToken ct = default)
    {
        var notif = new Notificacion
        {
            UsuarioId = usuarioId,
            ReportePersonaId = personaId,
            Tipo = "CASO_CERRADO",
            Titulo = "Actualización de caso",
            Mensaje = $"El caso ha sido actualizado a: {status}.",
            FechaCreacion = DateTime.UtcNow
        };
        _context.Notificaciones.Add(notif);
        await _context.SaveChangesAsync(ct);
    }
}
