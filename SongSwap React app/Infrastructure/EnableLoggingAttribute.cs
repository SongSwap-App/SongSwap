using Microsoft.AspNetCore.Mvc.Filters;
using SongSwap_React_app.Controllers;
using System.Diagnostics;

namespace SongSwap_React_app.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EnableLoggingAttribute : Attribute, IAsyncActionFilter
    {
        

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Stopwatch sw = Stopwatch.StartNew();
            await next();
            sw.Stop();

            MonitoringController.logs.Add(new Tuple<DateTime, string, double>(DateTime.Now ,context.ActionDescriptor.DisplayName, sw.Elapsed.TotalSeconds));
        }
    }
}
