using DotNetConsistency.Api.Domain.Common;

namespace DotNetConsistency.Api.Application.Interfaces;

public interface IWriteRepository<T> where T : BaseEntity
{
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
