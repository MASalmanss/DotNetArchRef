using System.Linq.Expressions;

namespace DotNetArchRef.Application.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    IQueryable<T> ApplyOrdering(IQueryable<T> query);
}
