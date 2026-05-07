using DotNetConsistency.Domain.Common;

namespace DotNetConsistency.Application.Interfaces;

public interface IWriteRepository<T> where T : BaseEntity
{
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);
}
