using System.Text.RegularExpressions;
using DotNetConsistency.Domain.Exceptions;

namespace DotNetConsistency.Domain.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("E-posta adresi boş olamaz.");

        if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new DomainException("Geçerli bir e-posta adresi girilmelidir.");

        return new Email(value);
    }

    public static Email FromDatabase(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new DataCorruptionException(nameof(Email), value ?? "(null)");

        return new Email(value);
    }

    public override string ToString() => Value;
}
