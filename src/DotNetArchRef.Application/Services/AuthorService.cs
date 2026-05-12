using DotNetArchRef.Application.Common;
using DotNetArchRef.Application.DTOs;
using DotNetArchRef.Application.Interfaces;
using DotNetArchRef.Application.Mappers;
using DotNetArchRef.Application.Specifications;
using DotNetArchRef.Domain.Entities;
using DotNetArchRef.Domain.ValueObjects;

namespace DotNetArchRef.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IUnitOfWork _uow;

    public AuthorService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<IEnumerable<AuthorDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var authors = await _uow.Authors.GetAllAsync(ct);
        return Result<IEnumerable<AuthorDto>>.Ok(authors.Select(AuthorMapper.ToDto));
    }

    public async Task<Result<PagedResult<AuthorDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
    {
        var paged = await _uow.Authors.GetPagedAsync(query.Page, query.PageSize, ct);
        var dtos = paged.Items.Select(AuthorMapper.ToDto);
        return Result<PagedResult<AuthorDto>>.Ok(new PagedResult<AuthorDto>(dtos, paged.TotalCount, query.Page, query.PageSize));
    }

    public async Task<Result<PagedResult<AuthorDto>>> GetPagedAsync(ISpecification<Author> spec, PagedQuery query, CancellationToken ct = default)
    {
        var paged = await _uow.Authors.GetPagedAsync(spec, query.Page, query.PageSize, ct);
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
        var existing = await _uow.Authors.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            return Error.Conflict($"An author with email '{request.Email}' already exists.");

        var author = Author.Create(request.Name, Email.Create(request.Email));

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
