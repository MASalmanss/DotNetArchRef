using DotNetArchRef.Domain.Entities;
using DotNetArchRef.Domain.ValueObjects;
using DotNetArchRef.Infrastructure.Persistence;
using DotNetArchRef.Infrastructure.Repositories;
using DotNetArchRef.Integration.Tests.Helpers;
using FluentAssertions;

namespace DotNetArchRef.Integration.Tests.Repositories;

public class AuthorRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldPersistAuthor()
    {
        await using var context = TestDbContextFactory.Create();
        var repo = new AuthorRepository(context);
        var uow = new UnitOfWork(context, new BookRepository(context), repo);

        var author = Author.Create("Orhan Pamuk", Email.Create("orhan@pamuk.com"));
        await repo.AddAsync(author);
        await uow.CommitAsync();

        var saved = await repo.GetByIdAsync(author.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Orhan Pamuk");
        saved.Email.Value.Should().Be("orhan@pamuk.com");
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnAuthor_WhenEmailExists()
    {
        await using var context = TestDbContextFactory.Create();
        var repo = new AuthorRepository(context);
        var uow = new UnitOfWork(context, new BookRepository(context), repo);

        var author = Author.Create("Yaşar Kemal", Email.Create("yasar@kemal.com"));
        await repo.AddAsync(author);
        await uow.CommitAsync();

        var found = await repo.GetByEmailAsync("yasar@kemal.com");
        found.Should().NotBeNull();
        found!.Name.Should().Be("Yaşar Kemal");
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailNotExists()
    {
        await using var context = TestDbContextFactory.Create();
        var repo = new AuthorRepository(context);

        var found = await repo.GetByEmailAsync("yok@yok.com");
        found.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ShouldRemoveAuthor()
    {
        await using var context = TestDbContextFactory.Create();
        var repo = new AuthorRepository(context);
        var uow = new UnitOfWork(context, new BookRepository(context), repo);

        var author = Author.Create("Silinecek Yazar", Email.Create("silinecek@yazar.com"));
        await repo.AddAsync(author);
        await uow.CommitAsync();

        repo.Delete(author);
        await uow.CommitAsync();

        var deleted = await repo.GetByIdAsync(author.Id);
        deleted.Should().BeNull();
    }
}
