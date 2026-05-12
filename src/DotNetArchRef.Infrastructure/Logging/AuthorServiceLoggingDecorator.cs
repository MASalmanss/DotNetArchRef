using DotNetArchRef.Application.Common;
using DotNetArchRef.Application.DTOs;
using DotNetArchRef.Application.Services;
using DotNetArchRef.Application.Specifications;
using DotNetArchRef.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace DotNetArchRef.Infrastructure.Logging;

public class AuthorServiceLoggingDecorator : IAuthorService
{
    private readonly IAuthorService _inner;
    private readonly ILogger<AuthorServiceLoggingDecorator> _logger;

    public AuthorServiceLoggingDecorator(IAuthorService inner, ILogger<AuthorServiceLoggingDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<AuthorDto>>> GetAllAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("GetAllAuthors started");
        var result = await _inner.GetAllAsync(ct);
        if (result.IsSuccess)
            _logger.LogInformation("GetAllAuthors completed: {Count} authors returned", result.Value!.Count());
        else
            _logger.LogWarning("GetAllAuthors failed: {Error}", result.Error);
        return result;
    }

    public async Task<Result<AuthorDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("GetAuthorById started: {Id}", id);
        var result = await _inner.GetByIdAsync(id, ct);
        if (result.IsSuccess)
            _logger.LogInformation("GetAuthorById completed: {Id}", id);
        else
            _logger.LogWarning("GetAuthorById failed: {Id} - {Error}", id, result.Error);
        return result;
    }

    public async Task<Result<AuthorDto>> CreateAsync(CreateAuthorRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("CreateAuthor started: Name={Name}", request.Name);
        var result = await _inner.CreateAsync(request, ct);
        if (result.IsSuccess)
            _logger.LogInformation("CreateAuthor completed: Id={Id}", result.Value!.Id);
        else
            _logger.LogWarning("CreateAuthor failed: {Error}", result.Error);
        return result;
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("DeleteAuthor started: {Id}", id);
        var result = await _inner.DeleteAsync(id, ct);
        if (result.IsSuccess)
            _logger.LogInformation("DeleteAuthor completed: {Id}", id);
        else
            _logger.LogWarning("DeleteAuthor failed: {Id} - {Error}", id, result.Error);
        return result;
    }

    public Task<Result<PagedResult<AuthorDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => _inner.GetPagedAsync(query, ct);

    public Task<Result<PagedResult<AuthorDto>>> GetPagedAsync(ISpecification<Author> spec, PagedQuery query, CancellationToken ct = default)
        => _inner.GetPagedAsync(spec, query, ct);
}
