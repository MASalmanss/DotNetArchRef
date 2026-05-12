using DotNetArchRef.Domain.Common;
using DotNetArchRef.Domain.Events;
using DotNetArchRef.Infrastructure.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DotNetArchRef.Infrastructure.EventHandlers;

public class BookAddedEventHandler : IDomainEventHandler<BookAddedEvent>
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<BookAddedEventHandler> _logger;

    public BookAddedEventHandler(IMemoryCache cache, ILogger<BookAddedEventHandler> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task HandleAsync(BookAddedEvent domainEvent, CancellationToken ct = default)
    {
        // Kitap eklendi — liste ve yazar cache'i geçersiz
        _cache.Remove(CacheKeys.AllBooks);
        _cache.Remove(CacheKeys.AuthorById(domainEvent.Author.Id));

        _logger.LogInformation(
            "Domain Event: BookAdded — cache invalidated. AuthorId={AuthorId}, BookTitle={BookTitle}, ISBN={ISBN}",
            domainEvent.Author.Id,
            domainEvent.Book.Title,
            domainEvent.Book.ISBN.Value);

        return Task.CompletedTask;
    }
}
