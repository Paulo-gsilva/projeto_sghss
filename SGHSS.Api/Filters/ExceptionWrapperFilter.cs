using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SGHSS.Api.Filters;

public class ExceptionWrapperFilter: IExceptionFilter
{
    private readonly ILogger<ExceptionWrapperFilter> _logger;

    public ExceptionWrapperFilter(ILogger<ExceptionWrapperFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Erro na execução da ação");

        var result = new ObjectResult(new
        {
            success = false,
            statusCode = StatusCodes.Status500InternalServerError,
            error = "Ocorreu um erro ao processar sua requisição."
        })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.Result = result;
        context.ExceptionHandled = true;
    }
}