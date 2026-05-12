using DotNetArchRef.Api.ExceptionHandlers;
using DotNetArchRef.Api.Filters;
using DotNetArchRef.Api.Validators;
using FluentValidation;

namespace DotNetArchRef.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers(options =>
            options.Filters.Add<ValidationFilter>());
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();

        services.AddSingleton<IExceptionHandler, DataCorruptionExceptionHandler>();
        services.AddSingleton<IExceptionHandler, DomainExceptionHandler>();
        services.AddSingleton<IExceptionHandler, NotFoundExceptionHandler>();
        services.AddSingleton<IExceptionHandler, ConflictExceptionHandler>();
        services.AddSingleton<IExceptionHandler, DefaultExceptionHandler>();

        return services;
    }
}
