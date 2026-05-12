using DotNetArchRef.Application.DTOs;
using DotNetArchRef.Domain.Entities;

namespace DotNetArchRef.Application.Mappers;

public static class BookMapper
{
    public static BookDto ToDto(Book book, string authorName)
        => new(book.Id, book.Title, book.ISBN.Value, book.Price.Amount, book.AuthorId, authorName, book.CreatedAt);
}
