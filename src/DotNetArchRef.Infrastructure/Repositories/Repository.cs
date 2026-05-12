using DotNetArchRef.Application.Common;
using DotNetArchRef.Application.Interfaces;
using DotNetArchRef.Application.Specifications;
using DotNetArchRef.Domain.Common;
using DotNetArchRef.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DotNetArchRef.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _dbSet.FindAsync([id], ct);

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        => await _dbSet.AsNoTracking().ToListAsync(ct);

    public async Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var total = await _dbSet.CountAsync(ct);
        var items = await _dbSet.AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return new PagedResult<T>(items, total, page, pageSize);
    }

    public async Task<PagedResult<T>> GetPagedAsync(ISpecification<T> spec, int page, int pageSize, CancellationToken ct = default)
    {
        var query = ApplySpecification(_dbSet.AsNoTracking(), spec);
        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return new PagedResult<T>(items, total, page, pageSize);
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _dbSet.AddAsync(entity, ct);

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void Delete(T entity)
        => _dbSet.Remove(entity);

    private static IQueryable<T> ApplySpecification(IQueryable<T> query, ISpecification<T> spec)
    {
        if (spec.Criteria is not null)
            query = query.Where(spec.Criteria);

        query = spec.ApplyOrdering(query);

        return query;
    }
}
