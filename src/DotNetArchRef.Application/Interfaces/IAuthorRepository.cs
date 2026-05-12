using DotNetArchRef.Domain.Entities;

namespace DotNetArchRef.Application.Interfaces;

public interface IAuthorRepository : IRepository<Author>
{
    Task<Author?> GetByEmailAsync(string email, CancellationToken ct = default);
}
