namespace TodoSeUsa.Application.Features.LoanNotes;

public class LoanNoteErrors
{
    public static Error Failure() => Error.Failure(
    "LoanNotes.Failure",
    "Ocurrió un error inesperado.");
}