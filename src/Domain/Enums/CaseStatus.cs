namespace LostPeople.Domain.Enums;

public enum CaseStatus
{
    Recibido = 1,
    EnVerificacion = 2,
    CoincidenciaDetectada = 3,
    EnInvestigacion = 4,
    LocalizadoConVida = 5,
    LocalizadoFallecido = 6,
    CerradoSinResolver = 7
}
