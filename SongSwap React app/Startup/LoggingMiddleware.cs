using MassTransit;
using SharedModels;
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
        private readonly IHttpClientFactory _httpClientFactory;

        public LoggingMiddleware(RequestDelegate next, IHttpClientFactory factory)
        {
            _next = next;
            _httpClientFactory = factory;
        }

        public async Task Invoke(HttpContext context)
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
                catch (OperationCanceledException)
                {
                    error = "Request cancelled by client";
                }
                catch (Exception ex) 
                {
                    await HandleExceptionAsync(context, ex);
                    error = ex.ToString();
                }


                sw.Stop();
                var log = new ActionLog(DateTime.Now, action, arguments, sw.Elapsed.TotalSeconds, error, query);

                MonitoringController.Logs.Add(log);

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
                Message = exception.Message
            }.ToString());
        }

        private static async Task FireAndForgetHttpCall(HttpClient client, ActionLog log)
        {
            await Task.Run(async () =>
            {
               await client.PostAsync("http://loggingapp/api/logging", JsonContent.Create(log));
            });
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
        public string Message { get; set; } = string.Empty;
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
 
}
