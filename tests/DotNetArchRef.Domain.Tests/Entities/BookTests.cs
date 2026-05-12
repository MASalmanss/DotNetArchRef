using DotNetArchRef.Domain.Entities;
using DotNetArchRef.Domain.Exceptions;
using DotNetArchRef.Domain.ValueObjects;
using FluentAssertions;

namespace DotNetArchRef.Domain.Tests.Entities;

public class BookTests
{
    private static readonly ISBN ValidISBN = ISBN.Create("978-3-16-148410-0");
    private static readonly Money ValidPrice = Money.Create(29.99m);

    [Fact]
    public void Create_WithValidData_ReturnsBook()
    {
        var book = Book.Create("Clean Code", ValidISBN, ValidPrice, authorId: 1);

        book.Title.Should().Be("Clean Code");
        book.ISBN.Should().Be(ValidISBN);
        book.Price.Should().Be(ValidPrice);
        book.AuthorId.Should().Be(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyTitle_ThrowsDomainException(string? title)
    {
        var act = () => Book.Create(title!, ValidISBN, ValidPrice, authorId: 1);
        act.Should().Throw<DomainException>().WithMessage("Kitap başlığı boş olamaz.");
    }

    [Fact]
    public void Create_WithTitleExceeding500Chars_ThrowsDomainException()
    {
        var longTitle = new string('A', 501);
        var act = () => Book.Create(longTitle, ValidISBN, ValidPrice, authorId: 1);
        act.Should().Throw<DomainException>().WithMessage("*500 karakter*");
    }

    [Fact]
    public void UpdateDetails_WithValidData_UpdatesTitleAndPrice()
    {
        var book = Book.Create("Old Title", ValidISBN, ValidPrice, authorId: 1);
        var newPrice = Money.Create(49.99m);

        book.UpdateDetails("New Title", newPrice);

        book.Title.Should().Be("New Title");
        book.Price.Should().Be(newPrice);
    }

    [Fact]
    public void UpdateDetails_WithEmptyTitle_ThrowsDomainException()
    {
        var book = Book.Create("Valid Title", ValidISBN, ValidPrice, authorId: 1);
        var act = () => book.UpdateDetails("", ValidPrice);
        act.Should().Throw<DomainException>();
    }
}
