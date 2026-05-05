namespace DotNetConsistency.Api.Application.DTOs;

public record CreateAuthorRequest(
    string Name,
    string Email
);
