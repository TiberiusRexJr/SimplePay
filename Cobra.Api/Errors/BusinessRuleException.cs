namespace Cobra.Api.Errors;

public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}
