using DotNetArchRef.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetArchRef.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IAuthorService, AuthorService>();

        return services;
    }
}
