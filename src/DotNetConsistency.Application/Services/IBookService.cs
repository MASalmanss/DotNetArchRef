using DotNetConsistency.Application.DTOs;

namespace DotNetConsistency.Application.Services;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllAsync(CancellationToken ct = default);
    Task<BookDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken ct = default);
    Task<BookDto> UpdateAsync(int id, UpdateBookRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
