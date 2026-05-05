using DotNetConsistency.Domain.Entities;

namespace DotNetConsistency.Application.Interfaces;

public interface IAuthorRepository : IRepository<Author>
{
    Task<Author?> GetByEmailAsync(string email, CancellationToken ct = default);
}
