namespace DotNetConsistency.Application.DTOs;

public record BookDto(
    int Id,
    string Title,
    string ISBN,
    decimal Price,
    int AuthorId,
    string AuthorName,
    DateTime CreatedAt
);
