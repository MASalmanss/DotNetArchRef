using DotNetConsistency.Application.DTOs;
using DotNetConsistency.Application.Mappers;
using DotNetConsistency.Domain.Entities;
using DotNetConsistency.Application.Interfaces;

namespace DotNetConsistency.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _books;
    private readonly IAuthorRepository _authors;

    public BookService(IBookRepository books, IAuthorRepository authors)
    {
        _books = books;
        _authors = authors;
    }

    public async Task<IEnumerable<BookDto>> GetAllAsync(CancellationToken ct = default)
    {
        var books = await _books.GetAllAsync(ct);
        return books.Select(b => BookMapper.ToDto(b, string.Empty));
    }

    public async Task<BookDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var book = await _books.GetByIdAsync(id, ct);
        if (book is null) return null;

        var author = await _authors.GetByIdAsync(book.AuthorId, ct);
        return BookMapper.ToDto(book, author?.Name ?? string.Empty);
    }

    public async Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken ct = default)
    {
        var author = await _authors.GetByIdAsync(request.AuthorId, ct)
            ?? throw new KeyNotFoundException($"Author with id {request.AuthorId} not found.");

        var existing = await _books.GetByIsbnAsync(request.ISBN, ct);
        if (existing is not null)
            throw new InvalidOperationException($"A book with ISBN '{request.ISBN}' already exists.");

        var book = new Book
        {
            Title = request.Title,
            ISBN = request.ISBN,
            Price = request.Price,
            AuthorId = request.AuthorId
        };

        await _books.AddAsync(book, ct);
        await _books.SaveChangesAsync(ct);

        return BookMapper.ToDto(book, author.Name);
    }

    public async Task<BookDto> UpdateAsync(int id, UpdateBookRequest request, CancellationToken ct = default)
    {
        var book = await _books.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Book with id {id} not found.");

        book.Title = request.Title;
        book.Price = request.Price;

        _books.Update(book);
        await _books.SaveChangesAsync(ct);

        var author = await _authors.GetByIdAsync(book.AuthorId, ct);
        return BookMapper.ToDto(book, author?.Name ?? string.Empty);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var book = await _books.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Book with id {id} not found.");

        _books.Delete(book);
        await _books.SaveChangesAsync(ct);
    }
}
