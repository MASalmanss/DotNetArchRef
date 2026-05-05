using DotNetConsistency.Api.Application.DTOs;
using DotNetConsistency.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetConsistency.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAll(CancellationToken ct)
    {
        var authors = await _authorService.GetAllAsync(ct);
        return Ok(authors);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuthorDto>> GetById(int id, CancellationToken ct)
    {
        var author = await _authorService.GetByIdAsync(id, ct);
        return author is null ? NotFound() : Ok(author);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> Create(CreateAuthorRequest request, CancellationToken ct)
    {
        var author = await _authorService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _authorService.DeleteAsync(id, ct);
        return NoContent();
    }
}
