using System.Linq.Expressions;

namespace DotNetConsistency.Application.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    bool IsDescending { get; }
}
