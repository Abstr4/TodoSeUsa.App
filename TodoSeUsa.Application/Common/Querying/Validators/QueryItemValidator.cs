namespace TodoSeUsa.Application.Common.Querying.Validators;

public class QueryItemValidator : AbstractValidator<QueryItem>
{
    public QueryItemValidator()
    {
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Skip debe ser mayor o igual a cero.");

        RuleFor(x => x.Take)
            .GreaterThan(0)
            .WithMessage("Take debe ser mayor que cero.")
            .LessThanOrEqualTo(100)
            .WithMessage("Take no debe excederse de 100.");
    }
}