namespace DotNetConsistency.Application.DTOs;

public record AuthorDto(
    int Id,
    string Name,
    string Email,
    DateTime CreatedAt
);
