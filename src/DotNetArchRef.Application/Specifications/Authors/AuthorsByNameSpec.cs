using DotNetArchRef.Domain.Entities;

namespace DotNetArchRef.Application.Specifications.Authors;

public class AuthorsByNameSpec : BaseSpecification<Author>
{
    private readonly string? _orderBy;
    private readonly bool _descending;

    public AuthorsByNameSpec(string? nameContains, string? orderBy = null, bool descending = false)
    {
        if (!string.IsNullOrWhiteSpace(nameContains))
            AddCriteria(a => a.Name.Contains(nameContains));

        _orderBy = orderBy?.ToLowerInvariant();
        _descending = descending;
    }

    public override IQueryable<Author> ApplyOrdering(IQueryable<Author> query)
        => (_orderBy, _descending) switch
        {
            ("name", false) => query.OrderBy(a => a.Name),
            ("name", true)  => query.OrderByDescending(a => a.Name),
            _               => query
        };
}
