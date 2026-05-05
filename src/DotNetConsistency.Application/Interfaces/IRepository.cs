using DotNetConsistency.Domain.Common;

namespace DotNetConsistency.Application.Interfaces;

public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T> where T : BaseEntity
{
}
