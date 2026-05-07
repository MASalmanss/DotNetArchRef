using DotNetConsistency.Application.Common;
using DotNetConsistency.Application.DTOs;
using DotNetConsistency.Application.Interfaces;
using DotNetConsistency.Application.Mappers;
using DotNetConsistency.Domain.Entities;
using FluentValidation;

namespace DotNetConsistency.Application.Services;

public class BookService : IBookService
{
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CreateBookRequest> _createValidator;
    private readonly IValidator<UpdateBookRequest> _updateValidator;

    public BookService(
        IUnitOfWork uow,
        IValidator<CreateBookRequest> createValidator,
        IValidator<UpdateBookRequest> updateValidator)
    {
        _uow = uow;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<IEnumerable<BookDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var books = (await _uow.Books.GetAllAsync(ct)).ToList();
        var authorMap = await BuildAuthorMapAsync(books.Select(b => b.AuthorId).Distinct(), ct);
        return Result<IEnumerable<BookDto>>.Ok(
            books.Select(b => BookMapper.ToDto(b, authorMap.GetValueOrDefault(b.AuthorId, string.Empty))));
    }

    public async Task<Result<PagedResult<BookDto>>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var paged = await _uow.Books.GetPagedAsync(page, pageSize, ct);
        var authorMap = await BuildAuthorMapAsync(paged.Items.Select(b => b.AuthorId).Distinct(), ct);
        var dtos = paged.Items.Select(b => BookMapper.ToDto(b, authorMap.GetValueOrDefault(b.AuthorId, string.Empty)));
        return Result<PagedResult<BookDto>>.Ok(new PagedResult<BookDto>(dtos, paged.TotalCount, page, pageSize));
    }

    public async Task<Result<BookDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var book = await _uow.Books.GetByIdAsync(id, ct);
        if (book is null)
            return Error.NotFound($"Book with id {id} not found.");

        var author = await _uow.Authors.GetByIdAsync(book.AuthorId, ct);
        return BookMapper.ToDto(book, author?.Name ?? string.Empty);
    }

    public async Task<Result<BookDto>> CreateAsync(CreateBookRequest request, CancellationToken ct = default)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Error.Validation(
                "One or more validation errors occurred.",
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        var author = await _uow.Authors.GetByIdAsync(request.AuthorId, ct);
        if (author is null)
            return Error.NotFound($"Author with id {request.AuthorId} not found.");

        var existing = await _uow.Books.GetByIsbnAsync(request.ISBN, ct);
        if (existing is not null)
            return Error.Conflict($"A book with ISBN '{request.ISBN}' already exists.");

        var book = new Book
        {
            Title = request.Title,
            ISBN = request.ISBN,
            Price = request.Price,
            AuthorId = request.AuthorId
        };

        await _uow.Books.AddAsync(book, ct);
        await _uow.CommitAsync(ct);

        return BookMapper.ToDto(book, author.Name);
    }

    public async Task<Result<BookDto>> UpdateAsync(int id, UpdateBookRequest request, CancellationToken ct = default)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Error.Validation(
                "One or more validation errors occurred.",
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        var book = await _uow.Books.GetByIdAsync(id, ct);
        if (book is null)
            return Error.NotFound($"Book with id {id} not found.");

        book.Title = request.Title;
        book.Price = request.Price;

        _uow.Books.Update(book);
        await _uow.CommitAsync(ct);

        var author = await _uow.Authors.GetByIdAsync(book.AuthorId, ct);
        return BookMapper.ToDto(book, author?.Name ?? string.Empty);
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
    {
        var book = await _uow.Books.GetByIdAsync(id, ct);
        if (book is null)
            return Result.Fail(Error.NotFound($"Book with id {id} not found."));

        _uow.Books.Delete(book);
        await _uow.CommitAsync(ct);

        return Result.Ok();
    }

    private async Task<Dictionary<int, string>> BuildAuthorMapAsync(IEnumerable<int> authorIds, CancellationToken ct)
    {
        var map = new Dictionary<int, string>();
        foreach (var id in authorIds)
        {
            var author = await _uow.Authors.GetByIdAsync(id, ct);
            map[id] = author?.Name ?? string.Empty;
        }
        return map;
    }
}
