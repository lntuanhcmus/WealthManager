using System.Runtime.Serialization;

namespace WealthManager.Shared.Exceptions;

public abstract class DomainException : Exception
{
    public DomainException() : base()
    {

    }
    public DomainException(string message) : base(message)
    {

    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {

    }

    protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context)
    {

    }
}