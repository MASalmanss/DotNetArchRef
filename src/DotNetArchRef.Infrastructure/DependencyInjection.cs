using DotNetArchRef.Application.Interfaces;
using DotNetArchRef.Application.Services;
using DotNetArchRef.Infrastructure.Cache;
using DotNetArchRef.Infrastructure.Data;
using DotNetArchRef.Infrastructure.Persistence;
using DotNetArchRef.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetArchRef.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddMemoryCache();

        services.AddScoped<BookService>();
        services.AddScoped<IBookService>(sp => new BookServiceCacheDecorator(
            sp.GetRequiredService<BookService>(),
            sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>()));

        services.AddScoped<AuthorService>();
        services.AddScoped<IAuthorService>(sp => new AuthorServiceCacheDecorator(
            sp.GetRequiredService<AuthorService>(),
            sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>()));

        return services;
    }
}
