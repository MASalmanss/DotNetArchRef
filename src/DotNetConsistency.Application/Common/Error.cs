namespace DotNetConsistency.Application.Common;

public sealed record Error(
    ErrorType Type,
    string Message,
    IReadOnlyList<string>? Details = null)
{
    public static Error NotFound(string message)
        => new(ErrorType.NotFound, message);

    public static Error Conflict(string message)
        => new(ErrorType.Conflict, message);

    public static Error Validation(string message, IReadOnlyList<string> details)
        => new(ErrorType.Validation, message, details);

    public static Error Unexpected(string message)
        => new(ErrorType.Unexpected, message);
}