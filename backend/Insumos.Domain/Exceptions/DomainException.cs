namespace Insumos.Domain.Exceptions;

/// <summary>
/// Represents a domain rule violation.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}

/// <summary>
/// Represents a conflict with an existing resource (e.g., duplicate code).
/// </summary>
public sealed class ConflictException : DomainException
{
    public ConflictException(string message) : base(message)
    {
    }
}

/// <summary>
/// Represents a missing resource.
/// </summary>
public sealed class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }
}
