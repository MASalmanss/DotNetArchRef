using DotNetConsistency.Application.Interfaces;
using DotNetConsistency.Domain.Entities;
using DotNetConsistency.Domain.ValueObjects;
using DotNetConsistency.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DotNetConsistency.Infrastructure.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(AppDbContext context) : base(context) { }

    public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken ct = default)
        => await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(b => b.ISBN == ISBN.Create(isbn), ct);

    public async Task<IEnumerable<Book>> GetByAuthorAsync(int authorId, CancellationToken ct = default)
        => await _dbSet.AsNoTracking()
            .Where(b => b.AuthorId == authorId)
            .ToListAsync(ct);
}
