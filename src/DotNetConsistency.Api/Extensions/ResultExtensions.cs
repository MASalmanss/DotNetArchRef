using DotNetConsistency.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace DotNetConsistency.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value);

        return MapError(result.Error!, controller);
    }

    public static IActionResult ToCreatedResult<T>(
        this Result<T> result,
        ControllerBase controller,
        string actionName,
        Func<T, object> routeValuesFactory)
    {
        if (result.IsSuccess)
            return controller.CreatedAtAction(actionName, routeValuesFactory(result.Value!), result.Value);

        return MapError(result.Error!, controller);
    }

    public static IActionResult ToNoContentResult(
        this Result result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.NoContent();

        return MapError(result.Error!, controller);
    }

    private static IActionResult MapError(Error error, ControllerBase controller)
        => error.Type switch
        {
            ErrorType.NotFound => controller.NotFound(new { error = error.Message }),
            ErrorType.Conflict => controller.Conflict(new { error = error.Message }),
            ErrorType.Validation => controller.UnprocessableEntity(new
            {
                error = error.Message,
                details = error.Details
            }),
            _ => controller.StatusCode(500, new { error = "An unexpected error occurred." })
        };
}
