using DotNetArchRef.Application.Common;
using DotNetArchRef.Application.DTOs;
using DotNetArchRef.Application.Specifications;
using DotNetArchRef.Domain.Entities;

namespace DotNetArchRef.Application.Services;

public interface IAuthorService
{
    Task<Result<IEnumerable<AuthorDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<PagedResult<AuthorDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default);
    Task<Result<PagedResult<AuthorDto>>> GetPagedAsync(ISpecification<Author> spec, PagedQuery query, CancellationToken ct = default);
    Task<Result<AuthorDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<AuthorDto>> CreateAsync(CreateAuthorRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(int id, CancellationToken ct = default);
}
