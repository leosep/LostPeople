using FluentValidation;

namespace LostPeople.Application.Coincidencias.Queries;

public class GetCoincidenciasQueryValidator : AbstractValidator<GetCoincidenciasQuery>
{
    public GetCoincidenciasQueryValidator()
    {
        RuleFor(v => v.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(v => v.PageSize)
            .InclusiveBetween(1, 100);
    }
}
