using DotNetArchRef.Domain.Entities;
using DotNetArchRef.Domain.ValueObjects;
using DotNetArchRef.Infrastructure.Persistence;
using DotNetArchRef.Infrastructure.Repositories;
using DotNetArchRef.Integration.Tests.Helpers;
using FluentAssertions;

namespace DotNetArchRef.Integration.Tests.Repositories;

public class BookRepositoryTests
{
    private static async Task<(Author author, AuthorRepository authorRepo, BookRepository bookRepo, UnitOfWork uow)>
        SetupAsync()
    {
        var context = TestDbContextFactory.Create();
        var authorRepo = new AuthorRepository(context);
        var bookRepo = new BookRepository(context);
        var uow = new UnitOfWork(context, bookRepo, authorRepo);

        var author = Author.Create("Test Yazar", Email.Create("test@yazar.com"));
        await authorRepo.AddAsync(author);
        await uow.CommitAsync();

        return (author, authorRepo, bookRepo, uow);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistBook()
    {
        var (author, _, bookRepo, uow) = await SetupAsync();

        var book = author.AddBook("Dune", ISBN.Create("978-0-441-17271-9"), Money.Create(89.90m));
        await bookRepo.AddAsync(book);
        await uow.CommitAsync();

        var saved = await bookRepo.GetByIdAsync(book.Id);
        saved.Should().NotBeNull();
        saved!.Title.Should().Be("Dune");
        saved.ISBN.Value.Should().Be("978-0-441-17271-9");
        saved.Price.Amount.Should().Be(89.90m);
    }

    [Fact]
    public async Task GetByIsbnAsync_ShouldReturnBook_WhenIsbnExists()
    {
        var (author, _, bookRepo, uow) = await SetupAsync();

        var book = author.AddBook("Foundation", ISBN.Create("978-0-553-29335-7"), Money.Create(75m));
        await bookRepo.AddAsync(book);
        await uow.CommitAsync();

        var found = await bookRepo.GetByIsbnAsync("978-0-553-29335-7");
        found.Should().NotBeNull();
        found!.Title.Should().Be("Foundation");
    }

    [Fact]
    public async Task GetByIsbnAsync_ShouldReturnNull_WhenIsbnNotExists()
    {
        var (_, _, bookRepo, _) = await SetupAsync();

        var found = await bookRepo.GetByIsbnAsync("000-0-000-00000-0");
        found.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBooks()
    {
        var (author, _, bookRepo, uow) = await SetupAsync();

        var book1 = author.AddBook("Kitap 1", ISBN.Create("111-1111111111"), Money.Create(50m));
        var book2 = author.AddBook("Kitap 2", ISBN.Create("222-2222222222"), Money.Create(60m));
        await bookRepo.AddAsync(book1);
        await bookRepo.AddAsync(book2);
        await uow.CommitAsync();

        var all = await bookRepo.GetAllAsync();
        all.Should().HaveCount(2);
    }
}
