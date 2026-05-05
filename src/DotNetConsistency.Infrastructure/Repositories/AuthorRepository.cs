using DotNetConsistency.Application.Interfaces;
using DotNetConsistency.Domain.Entities;
using DotNetConsistency.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DotNetConsistency.Infrastructure.Repositories;

public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(AppDbContext context) : base(context) { }

    public async Task<Author?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == email, ct);
}
