using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WealthManager.Shared.Models;

namespace WealthManager.Shared.Exceptions;

public class ValidationException : DomainException
{

    public IReadOnlyDictionary<string, string[]> Errors { get; }
    public ValidationException() : this("One or more validation errors occurred")
    {

    }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.Select(failure => failure.ErrorMessage).ToArray());
    }

    protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Errors = new Dictionary<string, string[]>();
    }


}
