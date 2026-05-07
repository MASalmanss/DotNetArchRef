using DotNetConsistency.Api.Extensions;
using DotNetConsistency.Application.DTOs;
using DotNetConsistency.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetConsistency.Api.Controllers;

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
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _bookService.GetPagedAsync(page, pageSize, ct);
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
