namespace DotNetArchRef.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Event'in tam tip adı — deserialize için gerekli
    public string EventType { get; set; } = string.Empty;

    // Event'in JSON'a serialize edilmiş hali
    public string Payload { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Null ise henüz işlenmedi, dolu ise işlendi
    public DateTime? ProcessedAt { get; set; }

    public string? Error { get; set; }
}
