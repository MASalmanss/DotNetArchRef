using DotNetConsistency.Application.DTOs;
using DotNetConsistency.Application.Mappers;
using DotNetConsistency.Domain.Entities;
using DotNetConsistency.Application.Interfaces;

namespace DotNetConsistency.Application.Services;

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
        return authors.Select(AuthorMapper.ToDto);
    }

    public async Task<AuthorDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var author = await _authors.GetByIdAsync(id, ct);
        return author is null ? null : AuthorMapper.ToDto(author);
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

        return AuthorMapper.ToDto(author);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var author = await _authors.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Author with id {id} not found.");

        _authors.Delete(author);
        await _authors.SaveChangesAsync(ct);
    }
}
