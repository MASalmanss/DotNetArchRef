using DotNetConsistency.Application.Common;
using DotNetConsistency.Domain.Common;

namespace DotNetConsistency.Application.Interfaces;

public interface IReadRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
}
