using DotNetConsistency.Api.Application.Interfaces;
using DotNetConsistency.Api.Infrastructure.Data;
using DotNetConsistency.Api.Infrastructure.ExceptionHandlers;
using DotNetConsistency.Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetConsistency.Api.Infrastructure;

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

        services.AddSingleton<IExceptionHandler, NotFoundExceptionHandler>();
        services.AddSingleton<IExceptionHandler, ConflictExceptionHandler>();
        services.AddSingleton<IExceptionHandler, DefaultExceptionHandler>();

        return services;
    }
}
