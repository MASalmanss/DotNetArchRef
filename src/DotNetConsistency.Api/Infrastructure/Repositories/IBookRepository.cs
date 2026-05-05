using DotNetConsistency.Api.Domain.Entities;

namespace DotNetConsistency.Api.Infrastructure.Repositories;

public interface IBookRepository : IRepository<Book>
{
    Task<Book?> GetByIsbnAsync(string isbn, CancellationToken ct = default);
    Task<IEnumerable<Book>> GetByAuthorAsync(int authorId, CancellationToken ct = default);
}
