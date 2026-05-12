using DotNetArchRef.Domain.Common;
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

        return new Author
        {
            Name = name,
            Email = email
        };
    }
}
