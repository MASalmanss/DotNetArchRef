using DotNetConsistency.Application.Common;
using DotNetConsistency.Application.DTOs;
using DotNetConsistency.Application.Specifications;
using DotNetConsistency.Domain.Entities;

namespace DotNetConsistency.Application.Services;

public interface IBookService
{
    Task<Result<IEnumerable<BookDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<PagedResult<BookDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default);
    Task<Result<PagedResult<BookDto>>> GetPagedAsync(ISpecification<Book> spec, PagedQuery query, CancellationToken ct = default);
    Task<Result<BookDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<BookDto>> CreateAsync(CreateBookRequest request, CancellationToken ct = default);
    Task<Result<BookDto>> UpdateAsync(int id, UpdateBookRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(int id, CancellationToken ct = default);
}
