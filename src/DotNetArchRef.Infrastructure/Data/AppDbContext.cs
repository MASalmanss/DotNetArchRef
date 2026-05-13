using System.Text.Json;
using DotNetArchRef.Domain.Common;
using DotNetArchRef.Domain.Entities;
using DotNetArchRef.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace DotNetArchRef.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        WriteOutboxMessages();  // event'leri aynı transaction'da outbox'a yaz

        return await base.SaveChangesAsync(cancellationToken);
        // Dispatch buradan kaldırıldı — OutboxProcessor halleder
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
    };

    private void WriteOutboxMessages()
    {
        var events = ChangeTracker.Entries<BaseEntity>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            entry.Entity.ClearDomainEvents();

        var outboxMessages = events.Select(e => new OutboxMessage
        {
            EventType = e.GetType().AssemblyQualifiedName!,
            Payload   = JsonSerializer.Serialize(e, e.GetType(), SerializerOptions)
        });

        OutboxMessages.AddRange(outboxMessages);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;

            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}
