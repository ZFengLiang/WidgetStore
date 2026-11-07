using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WidgetStore.Domain.Common;

namespace WidgetStore.Api.Filters;

public class DomainExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is DomainException ex)
        {
            context.Result = new BadRequestObjectResult(new { error = ex.Message });
            context.ExceptionHandled = true;
        }
    }
}