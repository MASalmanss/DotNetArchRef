using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DotNetArchRef.Api.ExceptionHandlers;

public class ConflictExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
        => exception is InvalidOperationException or DbUpdateException;

    public (int StatusCode, string Message) Handle(Exception exception)
    {
        var message = exception is DbUpdateException
            ? "İşlem veri bütünlüğü kısıtlamasını ihlal ediyor."
            : exception.Message;

        return (StatusCodes.Status409Conflict, message);
    }
}
