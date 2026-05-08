using DotNetConsistency.Domain.Entities;

namespace DotNetConsistency.Application.Specifications.Books;

public class BooksByPriceRangeSpec : BaseSpecification<Book>
{
    public BooksByPriceRangeSpec(decimal? minPrice, decimal? maxPrice, string? orderBy = null, bool descending = false)
    {
        if (minPrice.HasValue && maxPrice.HasValue)
            AddCriteria(b => b.Price.Amount >= minPrice.Value && b.Price.Amount <= maxPrice.Value);
        else if (minPrice.HasValue)
            AddCriteria(b => b.Price.Amount >= minPrice.Value);
        else if (maxPrice.HasValue)
            AddCriteria(b => b.Price.Amount <= maxPrice.Value);

        var order = orderBy?.ToLowerInvariant();
        if (order == "price")
            AddOrderBy(b => b.Price.Amount, descending);
        else if (order == "title")
            AddOrderBy(b => b.Title, descending);
    }
}
