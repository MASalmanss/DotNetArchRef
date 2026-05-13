using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetArchRef.Domain.Common;
using DotNetArchRef.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DotNetArchRef.Infrastructure.Outbox;

public class OutboxProcessor
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve
    };

    private readonly AppDbContext _context;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        AppDbContext context,
        IDomainEventDispatcher dispatcher,
        ILogger<OutboxProcessor> logger)
    {
        _context = context;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task ProcessAsync(CancellationToken ct = default)
    {
        // İşlenmemiş mesajları al — en fazla 20 tane aynı anda
        var messages = await _context.OutboxMessages
            .Where(m => m.ProcessedAt == null && m.Error == null)
            .OrderBy(m => m.CreatedAt)
            .Take(20)
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            try
            {
                var eventType = Type.GetType(message.EventType);
                if (eventType is null)
                {
                    message.Error = $"Type not found: {message.EventType}";
                    continue;
                }

                var domainEvent = (IDomainEvent)JsonSerializer.Deserialize(message.Payload, eventType, SerializerOptions)!;
                await _dispatcher.DispatchAsync(domainEvent, ct);

                message.ProcessedAt = DateTime.UtcNow;
                _logger.LogInformation("Outbox: processed {EventType} — Id={Id}", eventType.Name, message.Id);
            }
            catch (Exception ex)
            {
                message.Error = ex.Message;
                _logger.LogError(ex, "Outbox: failed to process message Id={Id}", message.Id);
            }
        }

        await _context.SaveChangesAsync(ct);
    }
}
