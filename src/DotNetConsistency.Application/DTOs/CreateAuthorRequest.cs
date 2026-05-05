namespace DotNetConsistency.Application.DTOs;

public record CreateAuthorRequest(
    string Name,
    string Email
);
