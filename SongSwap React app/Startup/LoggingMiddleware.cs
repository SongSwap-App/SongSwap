using SongSwap_React_app.Controllers;
using SongSwap_React_app.Infrastructure;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Startup
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IHttpClientFactory clientFactory)
        {
            string? controller = null;
            context.Request.RouteValues.TryGetValue("controller", out object? controllerObj);

            if (controllerObj != null)
            {
                controller = controllerObj.ToString();
            }

            string? error = null;

            if (controller != null && controller != "Monitoring")
            {
                string action = $"Controller: {context.Request.RouteValues["controller"]}, Action: {context.Request.RouteValues["action"]}";
                string? arguments = string.Join(", ", context.Request.RouteValues.Where(kv => kv.Key != "controller" && kv.Key != "action").Select(kv => $"{kv.Key}: {kv.Value}"));
                string? query = string.Join(", ", context.Request.Query.Select(kv => $"{kv.Key}: {kv.Value}"));

                var sw = Stopwatch.StartNew();
                try
                {
                    await _next(context);
                }
                catch (Exception ex) 
                {
                    await HandleExceptionAsync(context, ex);
                    error = ex.ToString();
                }


                sw.Stop();

                var log = new ActionLog(DateTime.Now, action, arguments, sw.Elapsed.TotalSeconds, error, query);
                MonitoringController.Logs.Add(log);

                var client = clientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"https://loggingapp/api/logging")
                {
                    Content = JsonContent.Create(log)
                };
                //try
                //{
                //    await client.SendAsync(request);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.ToString());
                //}
            }
            else
            {
                await _next(context);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error from the custom middleware."
            }.ToString());
        }
    }

    public static class LoggingMiddlewareExtension
    {
        public static IApplicationBuilder UseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }

    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
