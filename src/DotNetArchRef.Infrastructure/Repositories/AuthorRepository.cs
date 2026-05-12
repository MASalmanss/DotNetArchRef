using DotNetArchRef.Application.Interfaces;
using DotNetArchRef.Domain.Entities;
using DotNetArchRef.Domain.ValueObjects;
using DotNetArchRef.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DotNetArchRef.Infrastructure.Repositories;

public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(AppDbContext context) : base(context) { }

    public async Task<Author?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == Email.Create(email), ct);
}
