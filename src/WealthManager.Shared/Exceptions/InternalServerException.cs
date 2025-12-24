namespace WealthManager.Shared.Exceptions;

/// <summary>
/// Exception for internal server errors (500 Internal Server Error)
/// Used when system/infrastructure failures occur
/// </summary>
public class InternalServerException : DomainException
{
    public InternalServerException() : base("An internal server error occurred")
    {
    }

    public InternalServerException(string message) : base(message)
    {
    }

    public InternalServerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
