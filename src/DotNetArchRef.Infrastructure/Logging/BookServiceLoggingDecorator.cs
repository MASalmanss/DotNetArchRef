using DotNetArchRef.Application.Common;
using DotNetArchRef.Application.DTOs;
using DotNetArchRef.Application.Services;
using DotNetArchRef.Application.Specifications;
using DotNetArchRef.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace DotNetArchRef.Infrastructure.Logging;

public class BookServiceLoggingDecorator : IBookService
{
    private readonly IBookService _inner;
    private readonly ILogger<BookServiceLoggingDecorator> _logger;

    public BookServiceLoggingDecorator(IBookService inner, ILogger<BookServiceLoggingDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<BookDto>>> GetAllAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("GetAllBooks started");
        var result = await _inner.GetAllAsync(ct);
        if (result.IsSuccess)
            _logger.LogInformation("GetAllBooks completed: {Count} books returned", result.Value!.Count());
        else
            _logger.LogWarning("GetAllBooks failed: {Error}", result.Error);
        return result;
    }

    public async Task<Result<BookDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("GetBookById started: {Id}", id);
        var result = await _inner.GetByIdAsync(id, ct);
        if (result.IsSuccess)
            _logger.LogInformation("GetBookById completed: {Id}", id);
        else
            _logger.LogWarning("GetBookById failed: {Id} - {Error}", id, result.Error);
        return result;
    }

    public async Task<Result<BookDto>> CreateAsync(CreateBookRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("CreateBook started: Title={Title}, AuthorId={AuthorId}", request.Title, request.AuthorId);
        var result = await _inner.CreateAsync(request, ct);
        if (result.IsSuccess)
            _logger.LogInformation("CreateBook completed: Id={Id}", result.Value!.Id);
        else
            _logger.LogWarning("CreateBook failed: {Error}", result.Error);
        return result;
    }

    public async Task<Result<BookDto>> UpdateAsync(int id, UpdateBookRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("UpdateBook started: {Id}", id);
        var result = await _inner.UpdateAsync(id, request, ct);
        if (result.IsSuccess)
            _logger.LogInformation("UpdateBook completed: {Id}", id);
        else
            _logger.LogWarning("UpdateBook failed: {Id} - {Error}", id, result.Error);
        return result;
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("DeleteBook started: {Id}", id);
        var result = await _inner.DeleteAsync(id, ct);
        if (result.IsSuccess)
            _logger.LogInformation("DeleteBook completed: {Id}", id);
        else
            _logger.LogWarning("DeleteBook failed: {Id} - {Error}", id, result.Error);
        return result;
    }

    public Task<Result<PagedResult<BookDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => _inner.GetPagedAsync(query, ct);

    public Task<Result<PagedResult<BookDto>>> GetPagedAsync(ISpecification<Book> spec, PagedQuery query, CancellationToken ct = default)
        => _inner.GetPagedAsync(spec, query, ct);
}
