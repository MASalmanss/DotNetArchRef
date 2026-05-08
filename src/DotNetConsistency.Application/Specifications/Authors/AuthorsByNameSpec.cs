using DotNetConsistency.Domain.Entities;

namespace DotNetConsistency.Application.Specifications.Authors;

public class AuthorsByNameSpec : BaseSpecification<Author>
{
    public AuthorsByNameSpec(string? nameContains, string? orderBy = null, bool descending = false)
    {
        if (!string.IsNullOrWhiteSpace(nameContains))
            AddCriteria(a => a.Name.Contains(nameContains));

        var order = orderBy?.ToLowerInvariant();
        if (order == "name")
            AddOrderBy(a => a.Name, descending);
    }
}
