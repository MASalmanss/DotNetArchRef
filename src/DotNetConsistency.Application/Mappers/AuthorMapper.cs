using DotNetConsistency.Application.DTOs;
using DotNetConsistency.Domain.Entities;

namespace DotNetConsistency.Application.Mappers;

public static class AuthorMapper
{
    public static AuthorDto ToDto(Author author)
        => new(author.Id, author.Name, author.Email.Value, author.CreatedAt);
}
