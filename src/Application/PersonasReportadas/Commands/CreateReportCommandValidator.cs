using FluentValidation;

namespace LostPeople.Application.PersonasReportadas.Commands;

public class CreateReportCommandValidator : AbstractValidator<CreateReportCommand>
{
    public CreateReportCommandValidator()
    {
        RuleFor(v => v.PrimerNombre)
            .NotEmpty().WithMessage("El primer nombre es obligatorio")
            .MaximumLength(100);

        RuleFor(v => v.PrimerApellido)
            .NotEmpty().WithMessage("El primer apellido es obligatorio")
            .MaximumLength(100);

        RuleFor(v => v.SegundoNombre)
            .MaximumLength(100);

        RuleFor(v => v.SegundoApellido)
            .MaximumLength(100);

        RuleFor(v => v.Alias)
            .MaximumLength(100);

        RuleFor(v => v.Sexo)
            .MaximumLength(20)
            .Must(s => s == null || s == "M" || s == "F" || s == "Masculino" || s == "Femenino" || s == "Otro")
            .WithMessage("Sexo debe ser M, F, Masculino, Femenino u Otro");

        RuleFor(v => v.EdadAproximada)
            .InclusiveBetween(0, 150).When(v => v.EdadAproximada.HasValue)
            .WithMessage("Edad aproximada debe estar entre 0 y 150");

        RuleFor(v => v.EstaturaCm)
            .InclusiveBetween(20, 250).When(v => v.EstaturaCm.HasValue)
            .WithMessage("Estatura debe estar entre 20 y 250 cm");

        RuleFor(v => v.TelefonoContacto)
            .MaximumLength(20);

        RuleFor(v => v.EmailContacto)
            .EmailAddress().When(v => !string.IsNullOrEmpty(v.EmailContacto))
            .MaximumLength(200);

        RuleFor(v => v.AceptoTerminos)
            .Equal(true).WithMessage("Debes aceptar los términos y condiciones");

        RuleFor(v => v.AceptoConfidencialidad)
            .Equal(true).WithMessage("Debes aceptar la política de confidencialidad");
    }
}
