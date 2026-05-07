using DotNetConsistency.Application.Common;
using DotNetConsistency.Application.DTOs;

namespace DotNetConsistency.Application.Services;

public interface IAuthorService
{
    Task<Result<IEnumerable<AuthorDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<PagedResult<AuthorDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default);
    Task<Result<AuthorDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<AuthorDto>> CreateAsync(CreateAuthorRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(int id, CancellationToken ct = default);
}
