using DotNetConsistency.Domain.Exceptions;

namespace DotNetConsistency.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }

    private Money(decimal amount) => Amount = amount;

    public static Money Create(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Fiyat 0'dan büyük olmalıdır.");

        return new Money(amount);
    }

    public static Money FromDatabase(decimal amount)
    {
        if (amount <= 0)
            throw new DataCorruptionException(nameof(Money), amount.ToString());

        return new Money(amount);
    }

    public override string ToString() => Amount.ToString("F2");
}
