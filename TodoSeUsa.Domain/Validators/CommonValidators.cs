namespace TodoSeUsa.Domain.Validators;

public class CommonValidators
{
    public static void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }
    }

    public static void ValidateNaturalNumber(int number)
    {
        if (number < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(number), "Number must be zero or greater.");
        }
    }

    public static void ValidateDate(DateTime date)
    {
        if (date == default)
        {
            throw new ArgumentOutOfRangeException(nameof(date), "Date must be a valid date.");
        }
    }
}