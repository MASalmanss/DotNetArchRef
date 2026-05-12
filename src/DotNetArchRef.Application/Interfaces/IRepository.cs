using DotNetArchRef.Domain.Common;

namespace DotNetArchRef.Application.Interfaces;

public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T> where T : BaseEntity
{
}
