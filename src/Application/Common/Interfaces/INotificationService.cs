namespace LostPeople.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendSmsAsync(string toPhone, string message, CancellationToken ct = default);
    Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken ct = default);
    Task NotifyMatchFoundAsync(int usuarioId, int personaId, decimal score, CancellationToken ct = default);
    Task NotifyCaseClosedAsync(int usuarioId, int personaId, string status, CancellationToken ct = default);
}
