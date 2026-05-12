using DotNetArchRef.Domain.Common;
using DotNetArchRef.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DotNetArchRef.Infrastructure.EventHandlers;

public class AuthorCreatedEventHandler : IDomainEventHandler<AuthorCreatedEvent>
{
    private readonly ILogger<AuthorCreatedEventHandler> _logger;

    public AuthorCreatedEventHandler(ILogger<AuthorCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(AuthorCreatedEvent domainEvent, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Domain Event: AuthorCreated — Id={Id}, Name={Name}, Email={Email}",
            domainEvent.Author.Id,
            domainEvent.Author.Name,
            domainEvent.Author.Email.Value);

        return Task.CompletedTask;
    }
}
