using DotNetArchRef.Application.Common;
using DotNetArchRef.Application.DTOs;
using DotNetArchRef.Application.Services;
using DotNetArchRef.Application.Specifications;
using DotNetArchRef.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetArchRef.Infrastructure.Cache;

public class AuthorServiceCacheDecorator : IAuthorService
{
    private readonly IAuthorService _inner;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(5);

    public AuthorServiceCacheDecorator(IAuthorService inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<Result<IEnumerable<AuthorDto>>> GetAllAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(CacheKeys.AllAuthors, out Result<IEnumerable<AuthorDto>>? cached) && cached is not null)
            return cached;

        var result = await _inner.GetAllAsync(ct);

        if (result.IsSuccess)
            _cache.Set(CacheKeys.AllAuthors, result, DefaultExpiry);

        return result;
    }

    public async Task<Result<AuthorDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var key = CacheKeys.AuthorById(id);

        if (_cache.TryGetValue(key, out Result<AuthorDto>? cached) && cached is not null)
            return cached;

        var result = await _inner.GetByIdAsync(id, ct);

        if (result.IsSuccess)
            _cache.Set(key, result, DefaultExpiry);

        return result;
    }

    public async Task<Result<AuthorDto>> CreateAsync(CreateAuthorRequest request, CancellationToken ct = default)
    {
        var result = await _inner.CreateAsync(request, ct);

        if (result.IsSuccess)
            _cache.Remove(CacheKeys.AllAuthors);

        return result;
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _inner.DeleteAsync(id, ct);

        if (result.IsSuccess)
        {
            _cache.Remove(CacheKeys.AllAuthors);
            _cache.Remove(CacheKeys.AuthorById(id));
        }

        return result;
    }

    // Sayfalama ve filtreleme cache'lenmiyor.
    public Task<Result<PagedResult<AuthorDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => _inner.GetPagedAsync(query, ct);

    public Task<Result<PagedResult<AuthorDto>>> GetPagedAsync(ISpecification<Author> spec, PagedQuery query, CancellationToken ct = default)
        => _inner.GetPagedAsync(spec, query, ct);
}
