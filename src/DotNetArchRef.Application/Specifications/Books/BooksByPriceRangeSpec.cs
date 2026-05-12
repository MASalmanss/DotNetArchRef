using DotNetArchRef.Domain.Entities;

namespace DotNetArchRef.Application.Specifications.Books;

public class BooksByPriceRangeSpec : BaseSpecification<Book>
{
    private readonly string? _orderBy;
    private readonly bool _descending;

    public BooksByPriceRangeSpec(decimal? minPrice, decimal? maxPrice, string? orderBy = null, bool descending = false)
    {
        if (minPrice.HasValue && maxPrice.HasValue)
            AddCriteria(b => b.Price.Amount >= minPrice.Value && b.Price.Amount <= maxPrice.Value);
        else if (minPrice.HasValue)
            AddCriteria(b => b.Price.Amount >= minPrice.Value);
        else if (maxPrice.HasValue)
            AddCriteria(b => b.Price.Amount <= maxPrice.Value);

        _orderBy = orderBy?.ToLowerInvariant();
        _descending = descending;
    }

    public override IQueryable<Book> ApplyOrdering(IQueryable<Book> query)
        => (_orderBy, _descending) switch
        {
            ("price", false) => query.OrderBy(b => b.Price.Amount),
            ("price", true)  => query.OrderByDescending(b => b.Price.Amount),
            ("title", false) => query.OrderBy(b => b.Title),
            ("title", true)  => query.OrderByDescending(b => b.Title),
            _                => query
        };
}
