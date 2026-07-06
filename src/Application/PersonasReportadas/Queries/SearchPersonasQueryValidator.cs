using FluentValidation;

namespace LostPeople.Application.PersonasReportadas.Queries;

public class SearchPersonasQueryValidator : AbstractValidator<SearchPersonasQuery>
{
    public SearchPersonasQueryValidator()
    {
        RuleFor(v => v.Nombre)
            .MaximumLength(200);

        RuleFor(v => v.EdadDesde)
            .InclusiveBetween(0, 150).When(v => v.EdadDesde.HasValue);

        RuleFor(v => v.EdadHasta)
            .InclusiveBetween(0, 150).When(v => v.EdadHasta.HasValue);

        RuleFor(v => v.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(v => v.PageSize)
            .InclusiveBetween(1, 100);
    }
}
