using DotNetArchRef.Domain.Entities;
using DotNetArchRef.Domain.Exceptions;
using DotNetArchRef.Domain.ValueObjects;
using FluentAssertions;

namespace DotNetArchRef.Domain.Tests.Entities;

public class AuthorTests
{
    private static readonly Email ValidEmail = Email.Create("author@example.com");

    [Fact]
    public void Create_WithValidData_ReturnsAuthor()
    {
        var author = Author.Create("Robert Martin", ValidEmail);

        author.Name.Should().Be("Robert Martin");
        author.Email.Should().Be(ValidEmail);
        author.Books.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ThrowsDomainException(string? name)
    {
        var act = () => Author.Create(name!, ValidEmail);
        act.Should().Throw<DomainException>().WithMessage("Yazar adı boş olamaz.");
    }

    [Fact]
    public void Create_WithNameExceeding200Chars_ThrowsDomainException()
    {
        var longName = new string('A', 201);
        var act = () => Author.Create(longName, ValidEmail);
        act.Should().Throw<DomainException>().WithMessage("*200 karakter*");
    }

    [Fact]
    public void Books_IsReadOnly_CannotBeModifiedExternally()
    {
        var author = Author.Create("Robert Martin", ValidEmail);
        author.Books.Should().BeAssignableTo<IReadOnlyCollection<object>>();
    }
}
