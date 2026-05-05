using DotNetConsistency.Application;
using DotNetConsistency.Infrastructure;
using DotNetConsistency.Infrastructure.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    if (exception is null) return;

    var handlers = context.RequestServices.GetRequiredService<IEnumerable<IExceptionHandler>>();
    var handler = handlers.First(h => h.CanHandle(exception));
    var (status, message) = handler.Handle(exception);

    context.Response.StatusCode = status;
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(new { error = message });
}));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
