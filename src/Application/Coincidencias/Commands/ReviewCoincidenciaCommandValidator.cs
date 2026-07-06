using FluentValidation;

namespace LostPeople.Application.Coincidencias.Commands;

public class ReviewCoincidenciaCommandValidator : AbstractValidator<ReviewCoincidenciaCommand>
{
    public ReviewCoincidenciaCommandValidator()
    {
        RuleFor(v => v.CoincidenciaId)
            .GreaterThan(0);

        RuleFor(v => v.RevisorUsuarioId)
            .GreaterThan(0);

        RuleFor(v => v.Resultado)
            .NotEmpty().WithMessage("El resultado es obligatorio")
            .Must(r => r is "Confirmado" or "Descartado" or "EnVerificacion" or "FalsoPositivo")
            .WithMessage("Resultado debe ser Confirmado, Descartado, EnVerificacion o FalsoPositivo");

        RuleFor(v => v.Notas)
            .MaximumLength(2000);
    }
}
