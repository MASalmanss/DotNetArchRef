using System.Text.RegularExpressions;
using DotNetConsistency.Domain.Exceptions;

namespace DotNetConsistency.Domain.ValueObjects;

public sealed record ISBN
{
    public string Value { get; }

    private ISBN(string value) => Value = value;

    public static ISBN Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("ISBN boş olamaz.");

        if (!Regex.IsMatch(value, @"^[0-9\-]{10,20}$"))
            throw new DomainException("ISBN yalnızca rakam ve tire içermeli, 10-20 karakter uzunluğunda olmalıdır.");

        return new ISBN(value);
    }

    public static ISBN FromDatabase(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, @"^[0-9\-]{10,20}$"))
            throw new DataCorruptionException(nameof(ISBN), value ?? "(null)");

        return new ISBN(value);
    }

    public override string ToString() => Value;
}
