using DotNetConsistency.Api.Application.DTOs;
using DotNetConsistency.Api.Domain.Entities;
using DotNetConsistency.Api.Infrastructure.Repositories;

namespace DotNetConsistency.Api.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authors;

    public AuthorService(IAuthorRepository authors)
    {
        _authors = authors;
    }

    public async Task<IEnumerable<AuthorDto>> GetAllAsync(CancellationToken ct = default)
    {
        var authors = await _authors.GetAllAsync(ct);
        return authors.Select(MapToDto);
    }

    public async Task<AuthorDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var author = await _authors.GetByIdAsync(id, ct);
        return author is null ? null : MapToDto(author);
    }

    public async Task<AuthorDto> CreateAsync(CreateAuthorRequest request, CancellationToken ct = default)
    {
        var existing = await _authors.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            throw new InvalidOperationException($"An author with email '{request.Email}' already exists.");

        var author = new Author
        {
            Name = request.Name,
            Email = request.Email
        };

        await _authors.AddAsync(author, ct);
        await _authors.SaveChangesAsync(ct);

        return MapToDto(author);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var author = await _authors.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Author with id {id} not found.");

        _authors.Delete(author);
        await _authors.SaveChangesAsync(ct);
    }

    private static AuthorDto MapToDto(Author author)
        => new(author.Id, author.Name, author.Email, author.CreatedAt);
}
