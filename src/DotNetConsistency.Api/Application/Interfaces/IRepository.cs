using DotNetConsistency.Api.Domain.Common;

namespace DotNetConsistency.Api.Application.Interfaces;

public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T> where T : BaseEntity
{
}
