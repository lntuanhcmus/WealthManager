namespace WealthManager.Shared.Exceptions;

/// <summary>
/// Exception for authorization failures (403 Forbidden)
/// Used when user is authenticated but doesn't have permission to access resource
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException() : base("Access forbidden")
    {
    }

    public ForbiddenException(string message) : base(message)
    {
    }

    public ForbiddenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
