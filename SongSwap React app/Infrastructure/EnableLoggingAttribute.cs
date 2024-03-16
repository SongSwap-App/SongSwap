using Microsoft.AspNetCore.Mvc.Filters;
using SharedModels;
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

            var argument = string.Join(", ", context.ActionArguments.Select(kv => $"{kv.Key}: {kv.Value}"));

            MonitoringController.Logs.Add(new ActionLog(DateTime.Now ,context.ActionDescriptor.DisplayName!, argument, sw.ElapsedMilliseconds));
        }
    }
}
