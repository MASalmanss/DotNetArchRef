using DotNetArchRef.Api.Extensions;
using DotNetArchRef.Application.DTOs;
using DotNetArchRef.Application.Services;
using DotNetArchRef.Application.Specifications.Books;
using Microsoft.AspNetCore.Mvc;

namespace DotNetArchRef.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _bookService.GetAllAsync(ct);
        return result.ToActionResult(this);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
    {
        var result = await _bookService.GetPagedAsync(query, ct);
        return result.ToActionResult(this);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? orderBy,
        [FromQuery] bool descending = false,
        [FromQuery] PagedQuery? query = null,
        CancellationToken ct = default)
    {
        var spec = new BooksByPriceRangeSpec(minPrice, maxPrice, orderBy, descending);
        var result = await _bookService.GetPagedAsync(spec, query ?? new PagedQuery(), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _bookService.GetByIdAsync(id, ct);
        return result.ToActionResult(this);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookRequest request, CancellationToken ct)
    {
        var result = await _bookService.CreateAsync(request, ct);
        return result.ToCreatedResult(this, nameof(GetById), b => new { id = b.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateBookRequest request, CancellationToken ct)
    {
        var result = await _bookService.UpdateAsync(id, request, ct);
        return result.ToActionResult(this);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _bookService.DeleteAsync(id, ct);
        return result.ToNoContentResult(this);
    }
}
