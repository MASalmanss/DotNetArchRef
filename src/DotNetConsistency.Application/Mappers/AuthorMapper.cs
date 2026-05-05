using DotNetConsistency.Api.Application.DTOs;
using DotNetConsistency.Api.Domain.Entities;

namespace DotNetConsistency.Api.Application.Mappers;

public static class AuthorMapper
{
    public static AuthorDto ToDto(Author author)
        => new(author.Id, author.Name, author.Email, author.CreatedAt);
}
