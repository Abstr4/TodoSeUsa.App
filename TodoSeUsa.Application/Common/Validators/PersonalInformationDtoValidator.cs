public class PersonalInformationDtoValidator : AbstractValidator<PersonalInformationDto>
{
    public PersonalInformationDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .MustBeValidName("First name");

        RuleFor(x => x.LastName)
            .MustBeValidName("Last name");

        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.EmailAddress) ||
                !string.IsNullOrWhiteSpace(x.Address) ||
                !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("At least one contact method must be provided.");

        RuleFor(x => x.EmailAddress)
            .MustBeValidEmail()
            .When(x => !string.IsNullOrWhiteSpace(x.EmailAddress));

        RuleFor(x => x.PhoneNumber)
            .MustBeValidInternationalPhone()
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Address)
            .MustBeValidAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Address));
    }
}