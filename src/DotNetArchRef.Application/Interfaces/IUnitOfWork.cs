namespace DotNetArchRef.Application.Interfaces;

public interface IUnitOfWork
{
    IBookRepository Books { get; }
    IAuthorRepository Authors { get; }
    Task<int> CommitAsync(CancellationToken ct = default);
}
