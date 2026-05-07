using DotNetConsistency.Application.Common;
using DotNetConsistency.Application.DTOs;
using DotNetConsistency.Application.Interfaces;
using DotNetConsistency.Application.Mappers;
using DotNetConsistency.Domain.Entities;
using FluentValidation;

namespace DotNetConsistency.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CreateAuthorRequest> _createValidator;
    private readonly IValidator<PagedQuery> _pagedValidator;

    public AuthorService(
        IUnitOfWork uow,
        IValidator<CreateAuthorRequest> createValidator,
        IValidator<PagedQuery> pagedValidator)
    {
        _uow = uow;
        _createValidator = createValidator;
        _pagedValidator = pagedValidator;
    }

    public async Task<Result<IEnumerable<AuthorDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var authors = await _uow.Authors.GetAllAsync(ct);
        return Result<IEnumerable<AuthorDto>>.Ok(authors.Select(AuthorMapper.ToDto));
    }

    public async Task<Result<PagedResult<AuthorDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
    {
        var validation = await _pagedValidator.ValidateAsync(query, ct);
        if (!validation.IsValid)
            return Error.Validation(
                "Geçersiz sayfalama parametreleri.",
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        var paged = await _uow.Authors.GetPagedAsync(query.Page, query.PageSize, ct);
        var dtos = paged.Items.Select(AuthorMapper.ToDto);
        return Result<PagedResult<AuthorDto>>.Ok(new PagedResult<AuthorDto>(dtos, paged.TotalCount, query.Page, query.PageSize));
    }

    public async Task<Result<AuthorDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var author = await _uow.Authors.GetByIdAsync(id, ct);
        if (author is null)
            return Error.NotFound($"Author with id {id} not found.");

        return AuthorMapper.ToDto(author);
    }

    public async Task<Result<AuthorDto>> CreateAsync(CreateAuthorRequest request, CancellationToken ct = default)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Error.Validation(
                "One or more validation errors occurred.",
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        var existing = await _uow.Authors.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            return Error.Conflict($"An author with email '{request.Email}' already exists.");

        var author = new Author
        {
            Name = request.Name,
            Email = request.Email
        };

        await _uow.Authors.AddAsync(author, ct);
        await _uow.CommitAsync(ct);

        return AuthorMapper.ToDto(author);
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
    {
        var author = await _uow.Authors.GetByIdAsync(id, ct);
        if (author is null)
            return Result.Fail(Error.NotFound($"Author with id {id} not found."));

        _uow.Authors.Delete(author);
        await _uow.CommitAsync(ct);

        return Result.Ok();
    }
}
