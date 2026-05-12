using DotNetArchRef.Application.Common;
using DotNetArchRef.Application.Specifications;
using DotNetArchRef.Domain.Common;

namespace DotNetArchRef.Application.Interfaces;

public interface IReadRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<PagedResult<T>> GetPagedAsync(ISpecification<T> spec, int page, int pageSize, CancellationToken ct = default);
}
