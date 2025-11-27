using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SGHSS.Api.Filters;

public class ResponseWrapperFilter: IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult)
        {
            var originalStatusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;

            var responseBody = new
            {
                success = originalStatusCode is >= 200 and < 300,
                statusCode = originalStatusCode,
                data = objectResult.Value
            };

            context.Result = new ObjectResult(responseBody)
            {
                StatusCode = originalStatusCode
            };
        }

        await next();
    }
}