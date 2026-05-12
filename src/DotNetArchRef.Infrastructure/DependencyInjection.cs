using DotNetArchRef.Application.Interfaces;
using DotNetArchRef.Application.Services;
using DotNetArchRef.Domain.Common;
using DotNetArchRef.Domain.Events;
using DotNetArchRef.Infrastructure.Cache;
using DotNetArchRef.Infrastructure.Data;
using DotNetArchRef.Infrastructure.EventHandlers;
using DotNetArchRef.Infrastructure.Logging;
using DotNetArchRef.Infrastructure.Persistence;
using DotNetArchRef.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNetArchRef.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IDomainEventHandler<AuthorCreatedEvent>, AuthorCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<BookAddedEvent>, BookAddedEventHandler>();

        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddMemoryCache();

        services.AddScoped<BookService>();
        services.AddScoped<IBookService>(sp =>
        {
            var cache = sp.GetRequiredService<IMemoryCache>();
            var logger = sp.GetRequiredService<ILogger<BookServiceLoggingDecorator>>();

            IBookService cached = new BookServiceCacheDecorator(sp.GetRequiredService<BookService>(), cache);
            return new BookServiceLoggingDecorator(cached, logger);
        });

        services.AddScoped<AuthorService>();
        services.AddScoped<IAuthorService>(sp =>
        {
            var cache = sp.GetRequiredService<IMemoryCache>();
            var logger = sp.GetRequiredService<ILogger<AuthorServiceLoggingDecorator>>();

            IAuthorService cached = new AuthorServiceCacheDecorator(sp.GetRequiredService<AuthorService>(), cache);
            return new AuthorServiceLoggingDecorator(cached, logger);
        });

        return services;
    }
}
