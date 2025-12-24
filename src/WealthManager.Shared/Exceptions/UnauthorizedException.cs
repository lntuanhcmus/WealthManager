namespace WealthManager.Shared.Exceptions;

/// <summary>
/// Exception for authentication failures (401 Unauthorized)
/// Used when user credentials are invalid or authentication token is missing/invalid
/// </summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException() : base("Authentication failed")
    {
    }

    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
