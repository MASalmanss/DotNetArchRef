using DotNetArchRef.Domain.Exceptions;
using DotNetArchRef.Domain.ValueObjects;
using FluentAssertions;

namespace DotNetArchRef.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("name.surname@domain.org")]
    [InlineData("test+filter@mail.co")]
    public void Create_WithValidEmail_ReturnsEmail(string value)
    {
        var email = Email.Create(value);
        email.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyValue_ThrowsDomainException(string? value)
    {
        var act = () => Email.Create(value!);
        act.Should().Throw<DomainException>().WithMessage("*boş olamaz*");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@domain")]
    [InlineData("@nodomain.com")]
    public void Create_WithInvalidFormat_ThrowsDomainException(string value)
    {
        var act = () => Email.Create(value);
        act.Should().Throw<DomainException>().WithMessage("*geçerli bir e-posta*");
    }

    [Fact]
    public void TwoEmails_WithSameValue_AreEqual()
    {
        var a = Email.Create("user@example.com");
        var b = Email.Create("user@example.com");
        a.Should().Be(b);
    }

    [Fact]
    public void FromDatabase_WithInvalidValue_ThrowsDataCorruptionException()
    {
        var act = () => Email.FromDatabase("not-an-email");
        act.Should().Throw<DataCorruptionException>();
    }
}
