using DotNetArchRef.Domain.Common;
using DotNetArchRef.Domain.Events;
using DotNetArchRef.Domain.Exceptions;
using DotNetArchRef.Domain.ValueObjects;

namespace DotNetArchRef.Domain.Entities;

public class Author : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;

    private readonly List<Book> _books = [];
    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    private Author() { }

    public static Author Create(string name, Email email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Yazar adı boş olamaz.");

        if (name.Length > 200)
            throw new DomainException("Yazar adı en fazla 200 karakter olabilir.");

        var author = new Author
        {
            Name = name,
            Email = email
        };

        author.AddDomainEvent(new AuthorCreatedEvent(author));

        return author;
    }

    public Book AddBook(string title, ISBN isbn, Money price)
    {
        if (_books.Count >= 20)
            throw new DomainException("Bir yazar en fazla 20 kitaba sahip olabilir.");

        var book = Book.Create(title, isbn, price, Id);
        _books.Add(book);

        AddDomainEvent(new BookAddedEvent(this, book));

        return book;
    }
}
