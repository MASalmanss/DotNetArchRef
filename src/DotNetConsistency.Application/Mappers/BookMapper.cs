using DotNetConsistency.Api.Application.DTOs;
using DotNetConsistency.Api.Domain.Entities;

namespace DotNetConsistency.Api.Application.Mappers;

public static class BookMapper
{
    public static BookDto ToDto(Book book, string authorName)
        => new(book.Id, book.Title, book.ISBN, book.Price, book.AuthorId, authorName, book.CreatedAt);
}
