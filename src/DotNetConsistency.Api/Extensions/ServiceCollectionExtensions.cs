using DotNetConsistency.Api.Filters;

namespace DotNetConsistency.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers(options =>
            options.Filters.Add<ValidationFilter>());
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
}
