using DotNetConsistency.Domain.Exceptions;
using DotNetConsistency.Domain.ValueObjects;
using FluentAssertions;

namespace DotNetConsistency.Domain.Tests.ValueObjects;

public class ISBNTests
{
    [Theory]
    [InlineData("978-3-16-148410-0")]
    [InlineData("0-7475-3269-9")]
    [InlineData("9783161484100")]
    public void Create_WithValidValue_ReturnsISBN(string value)
    {
        var isbn = ISBN.Create(value);
        isbn.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyValue_ThrowsDomainException(string? value)
    {
        var act = () => ISBN.Create(value!);
        act.Should().Throw<DomainException>().WithMessage("ISBN boş olamaz.");
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("123")]
    [InlineData("isbn-not-valid!")]
    public void Create_WithInvalidFormat_ThrowsDomainException(string value)
    {
        var act = () => ISBN.Create(value);
        act.Should().Throw<DomainException>().WithMessage("*rakam ve tire*");
    }

    [Fact]
    public void TwoISBNs_WithSameValue_AreEqual()
    {
        var a = ISBN.Create("978-3-16-148410-0");
        var b = ISBN.Create("978-3-16-148410-0");
        a.Should().Be(b);
    }

    [Fact]
    public void TwoISBNs_WithDifferentValues_AreNotEqual()
    {
        var a = ISBN.Create("978-3-16-148410-0");
        var b = ISBN.Create("0-7475-3269-9");
        a.Should().NotBe(b);
    }

    [Fact]
    public void FromDatabase_WithInvalidValue_ThrowsDataCorruptionException()
    {
        var act = () => ISBN.FromDatabase("corrupted!");
        act.Should().Throw<DataCorruptionException>().WithMessage("*bozuk veri*");
    }

    [Fact]
    public void FromDatabase_WithValidValue_ReturnsISBN()
    {
        var isbn = ISBN.FromDatabase("978-3-16-148410-0");
        isbn.Value.Should().Be("978-3-16-148410-0");
    }
}
