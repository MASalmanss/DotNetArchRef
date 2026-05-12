using Microsoft.AspNetCore.Diagnostics;
using IExceptionHandler = DotNetArchRef.Api.ExceptionHandlers.IExceptionHandler;

namespace DotNetArchRef.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (exception is null) return;

            var handlers = context.RequestServices.GetRequiredService<IEnumerable<IExceptionHandler>>();
            var handler = handlers.First(h => h.CanHandle(exception));
            var (status, message) = handler.Handle(exception);

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = message });
        }));
    }

    public static IApplicationBuilder UseSwaggerInDevelopment(this IApplicationBuilder app)
    {
        if (app is WebApplication webApp && webApp.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        return app;
    }
}
