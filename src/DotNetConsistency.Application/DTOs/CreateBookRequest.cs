namespace DotNetConsistency.Application.DTOs;

public record CreateBookRequest(
    string Title,
    string ISBN,
    decimal Price,
    int AuthorId
);
