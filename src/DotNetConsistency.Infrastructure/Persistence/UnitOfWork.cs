using DotNetConsistency.Application.Interfaces;
using DotNetConsistency.Infrastructure.Data;

namespace DotNetConsistency.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(
        AppDbContext context,
        IBookRepository books,
        IAuthorRepository authors)
    {
        _context = context;
        Books = books;
        Authors = authors;
    }

    public IBookRepository Books { get; }
    public IAuthorRepository Authors { get; }

    public Task<int> CommitAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
