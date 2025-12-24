namespace WealthManager.Shared.Exceptions;

/// <summary>
/// Exception for resource conflicts (409 Conflict)
/// Used when trying to create duplicate resources (e.g., email already exists)
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException() : base("A conflict occurred")
    {
    }

    public ConflictException(string message) : base(message)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
