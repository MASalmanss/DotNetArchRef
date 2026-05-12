using System.Linq.Expressions;

namespace DotNetArchRef.Application.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    protected void AddCriteria(Expression<Func<T, bool>> criteria)
        => Criteria = criteria;

    public virtual IQueryable<T> ApplyOrdering(IQueryable<T> query) => query;
}
