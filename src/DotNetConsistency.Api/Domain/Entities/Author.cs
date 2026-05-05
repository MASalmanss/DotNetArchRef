using DotNetConsistency.Api.Domain.Common;

namespace DotNetConsistency.Api.Domain.Entities;

public class Author : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
