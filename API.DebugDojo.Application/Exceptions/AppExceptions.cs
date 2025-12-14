namespace API.DebugDojo.Application.Exceptions;

public abstract class AppException : Exception
{
    protected AppException(string message) : base(message) { }
}

public sealed class NotFoundException : AppException { public NotFoundException(string m) : base(m) { } }
public sealed class ConflictException : AppException { public ConflictException(string m) : base(m) { } }
public sealed class UnauthorizedAppException : AppException { public UnauthorizedAppException(string m) : base(m) { } }
