namespace TechnicalChallenge.Shared.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public NotFoundException(string resourceName, Guid id)
        : base($"{resourceName} with ID '{id}' was not found.")
    {
    }
}
