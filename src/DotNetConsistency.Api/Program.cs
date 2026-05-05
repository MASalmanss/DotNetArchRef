using DotNetConsistency.Api.Application.Services;
using DotNetConsistency.Api.Infrastructure.Data;
using DotNetConsistency.Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    var (status, message) = exception switch
    {
        KeyNotFoundException e => (StatusCodes.Status404NotFound, e.Message),
        InvalidOperationException e => (StatusCodes.Status409Conflict, e.Message),
        DbUpdateException => (StatusCodes.Status409Conflict, "Operation violates a data integrity constraint."),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
    };
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
