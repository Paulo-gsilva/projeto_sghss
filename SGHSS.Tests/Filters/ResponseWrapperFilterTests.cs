using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SGHSS.Api.Filters;

namespace SGHSS.Tests.Filters;

public class ResponseWrapperFilterTests
{
    [Fact]
    public async Task OnResultExecutionAsync_ShouldWrapOkResponse()
    {
        ResponseWrapperFilter filter = new();

        DefaultHttpContext httpContext = new();
        var actionContext = new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
        );

        var objectResult = new ObjectResult(new { message = "OK" })
        {
            StatusCode = StatusCodes.Status200OK
        };

        ResultExecutingContext executingContext = new(
            actionContext,
            new List<IFilterMetadata>(),
            objectResult,
            controller: null
        );

        ResultExecutionDelegate next = () =>
            Task.FromResult(new ResultExecutedContext(actionContext, new List<IFilterMetadata>(), objectResult, null));

        await filter.OnResultExecutionAsync(executingContext, next);

        var result = Assert.IsType<ObjectResult>(executingContext.Result);

        Assert.Equal(200, result.StatusCode);
    }
}