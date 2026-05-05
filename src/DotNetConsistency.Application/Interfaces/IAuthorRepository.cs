using DotNetConsistency.Api.Domain.Entities;

namespace DotNetConsistency.Api.Application.Interfaces;

public interface IAuthorRepository : IRepository<Author>
{
    Task<Author?> GetByEmailAsync(string email, CancellationToken ct = default);
}
