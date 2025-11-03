namespace TodoSeUsa.Application.Entities.Common.Validators;

using FluentValidation;
using TodoSeUsa.Domain.Validators;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string?> MustBeValidInternationalPhone<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must((phone) => PhoneNumberValidator.TryValidate(phone, out _))
            .WithMessage("Phone number must be in a valid international format (E.164).");
    }

    public static IRuleBuilderOptions<T, string?> MustBeValidEmail<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must((email) => EmailAddressValidator.TryValidate(email, out _))
            .WithMessage("Email must be a valid email address.");
    }

    public static IRuleBuilderOptions<T, string?> MustBeValidAddress<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(200)
            .WithMessage("Address must not be empty and must be less than 200 characters.");
    }

    public static IRuleBuilderOptions<T, string?> MustBeValidName<T>(
        this IRuleBuilder<T, string?> ruleBuilder, string field = "Name")
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(70)
            .WithMessage($"{field} must be between 2 and 70 characters.");
    }

    public static IRuleBuilderOptions<T, int> MustBeNaturalNumber<T>(
        this IRuleBuilder<T, int> ruleBuilder, string field = "Number")
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(1)
            .WithMessage($"{field} must be a natural number (greater than or equal to 1).");
    }
}