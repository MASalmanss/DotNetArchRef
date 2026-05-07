using DotNetConsistency.Application.Services;
using DotNetConsistency.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetConsistency.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IAuthorService, AuthorService>();

        services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();

        return services;
    }
}
