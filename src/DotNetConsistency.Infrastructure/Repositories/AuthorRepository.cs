using DotNetConsistency.Api.Application.Interfaces;
using DotNetConsistency.Api.Domain.Entities;
using DotNetConsistency.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DotNetConsistency.Api.Infrastructure.Repositories;

public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(AppDbContext context) : base(context) { }

    public async Task<Author?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == email, ct);
}
