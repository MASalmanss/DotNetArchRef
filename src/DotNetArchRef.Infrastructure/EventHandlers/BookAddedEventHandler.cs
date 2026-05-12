using DotNetArchRef.Domain.Common;
using DotNetArchRef.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DotNetArchRef.Infrastructure.EventHandlers;

public class BookAddedEventHandler : IDomainEventHandler<BookAddedEvent>
{
    private readonly ILogger<BookAddedEventHandler> _logger;

    public BookAddedEventHandler(ILogger<BookAddedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(BookAddedEvent domainEvent, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Domain Event: BookAdded — AuthorId={AuthorId}, AuthorName={AuthorName}, BookTitle={BookTitle}, ISBN={ISBN}",
            domainEvent.Author.Id,
            domainEvent.Author.Name,
            domainEvent.Book.Title,
            domainEvent.Book.ISBN.Value);

        return Task.CompletedTask;
    }
}
