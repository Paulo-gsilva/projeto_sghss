using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Moq;
using SGHSS.Api.Filters;

namespace SGHSS.Tests.Filters;

[ExcludeFromCodeCoverage]
public class ExceptionWrapperFilterTests
{
    [Fact]
    public void OnException_ShouldReturnWrappedErrorResponse()
    {
        Mock<ILogger<ExceptionWrapperFilter>> loggerMock = new();
        ExceptionWrapperFilter filter = new(loggerMock.Object);

        DefaultHttpContext httpContext = new();
        ActionContext actionContext = new(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
        );

        ExceptionContext context = new(actionContext, new List<IFilterMetadata>())
        {
            Exception = new Exception("Falha")
        };

        filter.OnException(context);

        var objectResult = Assert.IsType<ObjectResult>(context.Result);

        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.True(context.ExceptionHandled);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, _) => true),
                context.Exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }
}