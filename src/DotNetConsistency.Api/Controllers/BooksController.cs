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
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAll(CancellationToken ct)
    {
        var books = await _bookService.GetAllAsync(ct);
        return Ok(books);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDto>> GetById(int id, CancellationToken ct)
    {
        var book = await _bookService.GetByIdAsync(id, ct);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create(CreateBookRequest request, CancellationToken ct)
    {
        var book = await _bookService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BookDto>> Update(int id, UpdateBookRequest request, CancellationToken ct)
    {
        var book = await _bookService.UpdateAsync(id, request, ct);
        return Ok(book);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _bookService.DeleteAsync(id, ct);
        return NoContent();
    }
}
