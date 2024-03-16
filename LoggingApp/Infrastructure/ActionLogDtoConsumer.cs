using LoggingApp.Controllers;
using LoggingApp.Infrastructure.Data;
using MassTransit;
using SharedModels;
using System.Text.Json;

namespace LoggingApp.Infrastructure
{
    public class ActionLogDtoConsumer : IConsumer<ActionLogDto>
    {
        private readonly LogsDbContext _context;

        public ActionLogDtoConsumer(LogsDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<ActionLogDto> context)
        {
            Console.WriteLine($"Processing log {context.Message.Action}");
            await Task.Delay(3000);
            LoggingController.Logs.Add(context.Message);
            Console.WriteLine($"{context.Message.Action} - Done");
        }
    }
}
