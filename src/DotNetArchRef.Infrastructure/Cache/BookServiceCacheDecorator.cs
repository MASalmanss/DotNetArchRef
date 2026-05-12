using DotNetArchRef.Application.Common;
using DotNetArchRef.Application.DTOs;
using DotNetArchRef.Application.Services;
using DotNetArchRef.Application.Specifications;
using DotNetArchRef.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetArchRef.Infrastructure.Cache;

public class BookServiceCacheDecorator : IBookService
{
    private readonly IBookService _inner;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(5);

    public BookServiceCacheDecorator(IBookService inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<Result<IEnumerable<BookDto>>> GetAllAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(CacheKeys.AllBooks, out Result<IEnumerable<BookDto>>? cached) && cached is not null)
            return cached;

        var result = await _inner.GetAllAsync(ct);

        if (result.IsSuccess)
            _cache.Set(CacheKeys.AllBooks, result, DefaultExpiry);

        return result;
    }

    public async Task<Result<BookDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var key = CacheKeys.BookById(id);

        if (_cache.TryGetValue(key, out Result<BookDto>? cached) && cached is not null)
            return cached;

        var result = await _inner.GetByIdAsync(id, ct);

        if (result.IsSuccess)
            _cache.Set(key, result, DefaultExpiry);

        return result;
    }

    public async Task<Result<BookDto>> CreateAsync(CreateBookRequest request, CancellationToken ct = default)
    {
        var result = await _inner.CreateAsync(request, ct);

        if (result.IsSuccess)
            _cache.Remove(CacheKeys.AllBooks);

        return result;
    }

    public async Task<Result<BookDto>> UpdateAsync(int id, UpdateBookRequest request, CancellationToken ct = default)
    {
        var result = await _inner.UpdateAsync(id, request, ct);

        if (result.IsSuccess)
        {
            _cache.Remove(CacheKeys.AllBooks);
            _cache.Remove(CacheKeys.BookById(id));
        }

        return result;
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _inner.DeleteAsync(id, ct);

        if (result.IsSuccess)
        {
            _cache.Remove(CacheKeys.AllBooks);
            _cache.Remove(CacheKeys.BookById(id));
        }

        return result;
    }

    // Sayfalama ve filtreleme sorgularını cache'lemiyoruz —
    // kombinasyon sayısı çok fazla, stale veri riski yüksek.
    public Task<Result<PagedResult<BookDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => _inner.GetPagedAsync(query, ct);

    public Task<Result<PagedResult<BookDto>>> GetPagedAsync(ISpecification<Book> spec, PagedQuery query, CancellationToken ct = default)
        => _inner.GetPagedAsync(spec, query, ct);
}
