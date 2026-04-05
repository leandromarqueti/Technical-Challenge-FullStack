namespace TechnicalChallenge.Shared.Exceptions.FluentValidation;

public class ValidationFailure
{
    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public object? AttemptedValue { get; set; }
}
