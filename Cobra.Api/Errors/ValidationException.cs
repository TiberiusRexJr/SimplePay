namespace Cobra.Api.Errors;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }
    public ValidationException(IDictionary<string, string[]> errors, string? message = null)
        : base(message ?? "Validation failed.") => Errors = errors;
}
