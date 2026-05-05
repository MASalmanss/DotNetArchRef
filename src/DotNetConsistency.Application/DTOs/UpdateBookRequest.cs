namespace DotNetConsistency.Application.DTOs;

public record UpdateBookRequest(
    string Title,
    decimal Price
);
