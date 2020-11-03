using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Roulette.Core.Services.Filters
{
    public class ErrorFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {

            Log.Error(context.Exception.ToString());
        }
    }
}
