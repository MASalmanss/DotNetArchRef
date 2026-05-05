namespace DotNetConsistency.Api.Application.DTOs;

public record UpdateBookRequest(
    string Title,
    decimal Price
);
