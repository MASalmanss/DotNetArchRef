namespace DotNetConsistency.Api.Application.DTOs;

public record CreateBookRequest(
    string Title,
    string ISBN,
    decimal Price,
    int AuthorId
);
