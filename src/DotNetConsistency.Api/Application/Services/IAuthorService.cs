using DotNetConsistency.Api.Application.DTOs;

namespace DotNetConsistency.Api.Application.Services;

public interface IAuthorService
{
    Task<IEnumerable<AuthorDto>> GetAllAsync(CancellationToken ct = default);
    Task<AuthorDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<AuthorDto> CreateAsync(CreateAuthorRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
