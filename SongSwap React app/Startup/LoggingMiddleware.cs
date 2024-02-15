using SongSwap_React_app.Controllers;
using SongSwap_React_app.Infrastructure;
using System.Diagnostics;
using System.Text;

namespace Startup
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string action = $"Controller: {context.Request.RouteValues["controller"]}, Action: {context.Request.RouteValues["action"]}";
            string? arguments = string.Join(", ", context.Request.RouteValues.Where(kv => kv.Key != "controller" && kv.Key != "action").Select(kv => $"{kv.Key}: {kv.Value}"));

            var sw = Stopwatch.StartNew();
            await _next(context);
            sw.Stop();

            var log = new ActionLog(DateTime.UtcNow, action, arguments, sw.Elapsed.TotalSeconds);
            MonitoringController.Logs.Add(log);
        }
    }

    public static class LoggingMiddlewareExtension
    {
        public static IApplicationBuilder UseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
