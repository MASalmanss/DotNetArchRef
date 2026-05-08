using DotNetConsistency.Api.Extensions;
using DotNetConsistency.Application.DTOs;
using DotNetConsistency.Application.Services;
using DotNetConsistency.Application.Specifications.Authors;
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
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _authorService.GetAllAsync(ct);
        return result.ToActionResult(this);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
    {
        var result = await _authorService.GetPagedAsync(query, ct);
        return result.ToActionResult(this);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? name,
        [FromQuery] string? orderBy,
        [FromQuery] bool descending = false,
        [FromQuery] PagedQuery? query = null,
        CancellationToken ct = default)
    {
        var spec = new AuthorsByNameSpec(name, orderBy, descending);
        var result = await _authorService.GetPagedAsync(spec, query ?? new PagedQuery(), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _authorService.GetByIdAsync(id, ct);
        return result.ToActionResult(this);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAuthorRequest request, CancellationToken ct)
    {
        var result = await _authorService.CreateAsync(request, ct);
        return result.ToCreatedResult(this, nameof(GetById), a => new { id = a.Id });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _authorService.DeleteAsync(id, ct);
        return result.ToNoContentResult(this);
    }
}
