using DotNetArchRef.Domain.Common;
using DotNetArchRef.Domain.Entities;

namespace DotNetArchRef.Domain.Events;

public record BookAddedEvent(Author Author, Book Book) : IDomainEvent;
