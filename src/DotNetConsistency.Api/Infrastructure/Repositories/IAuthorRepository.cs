using DotNetConsistency.Api.Domain.Entities;

namespace DotNetConsistency.Api.Infrastructure.Repositories;

public interface IAuthorRepository : IRepository<Author>
{
    Task<Author?> GetByEmailAsync(string email, CancellationToken ct = default);
}
