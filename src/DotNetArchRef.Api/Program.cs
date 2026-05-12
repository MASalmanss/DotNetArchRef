using DotNetArchRef.Api.Extensions;
using DotNetArchRef.Application;
using DotNetArchRef.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation();

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseSwaggerInDevelopment();
app.MapControllers();

app.Run();
