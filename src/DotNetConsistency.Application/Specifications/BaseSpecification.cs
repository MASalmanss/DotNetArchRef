using System.Linq.Expressions;

namespace DotNetConsistency.Application.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public bool IsDescending { get; private set; }

    protected void AddCriteria(Expression<Func<T, bool>> criteria)
        => Criteria = criteria;

    protected void AddOrderBy(Expression<Func<T, object>> orderBy, bool descending = false)
    {
        OrderBy = orderBy;
        IsDescending = descending;
    }
}
