using DotNetConsistency.Domain.Entities;
using DotNetConsistency.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetConsistency.Infrastructure.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(b => b.ISBN)
            .HasConversion(
                isbn => isbn.Value,
                value => ISBN.Create(value))
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(b => b.ISBN)
            .IsUnique();

        builder.Property(b => b.Price)
            .HasConversion(
                money => money.Amount,
                amount => Money.Create(amount))
            .HasPrecision(18, 2);
    }
}
