namespace DotNetArchRef.Application.DTOs;

public record UpdateBookRequest(
    string Title,
    decimal Price
);
