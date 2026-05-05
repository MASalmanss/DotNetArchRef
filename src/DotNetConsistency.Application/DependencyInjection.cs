using DotNetConsistency.Api.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetConsistency.Api.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IAuthorService, AuthorService>();
        return services;
    }
}
