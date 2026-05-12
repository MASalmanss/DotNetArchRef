using DotNetArchRef.Domain.Common;
using DotNetArchRef.Domain.Entities;

namespace DotNetArchRef.Domain.Events;

public record AuthorCreatedEvent(Author Author) : IDomainEvent;
