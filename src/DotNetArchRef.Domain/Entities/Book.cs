using DotNetArchRef.Domain.Common;
using DotNetArchRef.Domain.Exceptions;
using DotNetArchRef.Domain.ValueObjects;

namespace DotNetArchRef.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public ISBN ISBN { get; private set; } = null!;
    public Money Price { get; private set; } = null!;
    public int AuthorId { get; private set; }
    public Author Author { get; private set; } = null!;

    private Book() { }

    public static Book Create(string title, ISBN isbn, Money price, int authorId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Kitap başlığı boş olamaz.");

        if (title.Length > 500)
            throw new DomainException("Kitap başlığı en fazla 500 karakter olabilir.");

        return new Book
        {
            Title = title,
            ISBN = isbn,
            Price = price,
            AuthorId = authorId
        };
    }

    public void UpdateDetails(string title, Money price)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Kitap başlığı boş olamaz.");

        if (title.Length > 500)
            throw new DomainException("Kitap başlığı en fazla 500 karakter olabilir.");

        Title = title;
        Price = price;
    }
}
