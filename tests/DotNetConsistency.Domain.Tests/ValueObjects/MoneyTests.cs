using DotNetConsistency.Domain.Exceptions;
using DotNetConsistency.Domain.ValueObjects;
using FluentAssertions;

namespace DotNetConsistency.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Theory]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(999.99)]
    public void Create_WithPositiveAmount_ReturnsMoney(decimal amount)
    {
        var money = Money.Create(amount);
        money.Amount.Should().Be(amount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Create_WithZeroOrNegative_ThrowsDomainException(decimal amount)
    {
        var act = () => Money.Create(amount);
        act.Should().Throw<DomainException>().WithMessage("*0'dan büyük*");
    }

    [Fact]
    public void TwoMoneys_WithSameAmount_AreEqual()
    {
        var a = Money.Create(29.99m);
        var b = Money.Create(29.99m);
        a.Should().Be(b);
    }

    [Fact]
    public void FromDatabase_WithZeroAmount_ThrowsDataCorruptionException()
    {
        var act = () => Money.FromDatabase(0);
        act.Should().Throw<DataCorruptionException>();
    }

    [Fact]
    public void FromDatabase_WithValidAmount_ReturnsMoney()
    {
        var money = Money.FromDatabase(49.99m);
        money.Amount.Should().Be(49.99m);
    }
}
