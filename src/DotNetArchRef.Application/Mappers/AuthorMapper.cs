using DotNetArchRef.Application.DTOs;
using DotNetArchRef.Domain.Entities;

namespace DotNetArchRef.Application.Mappers;

public static class AuthorMapper
{
    public static AuthorDto ToDto(Author author)
        => new(author.Id, author.Name, author.Email.Value, author.CreatedAt);
}
