using DotNetConsistency.Domain.Entities;

namespace DotNetConsistency.Application.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<Book?> GetByIsbnAsync(string isbn, CancellationToken ct = default);
    Task<IEnumerable<Book>> GetByAuthorAsync(int authorId, CancellationToken ct = default);
}
