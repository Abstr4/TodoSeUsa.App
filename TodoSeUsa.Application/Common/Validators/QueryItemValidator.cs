namespace TodoSeUsa.Application.Common.Validators;

public class QueryItemValidator : AbstractValidator<QueryItem>
{
    public QueryItemValidator()
    {
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Skip must be greater than or equal to zero.");

        RuleFor(x => x.Take)
            .GreaterThan(0)
            .WithMessage("Take must be greater than zero.")
            .LessThanOrEqualTo(100)
            .WithMessage("Take must not exceed 100.");
    }
}
