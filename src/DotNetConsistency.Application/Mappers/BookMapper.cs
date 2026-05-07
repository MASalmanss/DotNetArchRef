using DotNetConsistency.Application.DTOs;
using DotNetConsistency.Domain.Entities;

namespace DotNetConsistency.Application.Mappers;

public static class BookMapper
{
    public static BookDto ToDto(Book book, string authorName)
        => new(book.Id, book.Title, book.ISBN.Value, book.Price.Amount, book.AuthorId, authorName, book.CreatedAt);
}
