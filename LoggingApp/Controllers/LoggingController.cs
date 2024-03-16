using Microsoft.AspNetCore.Mvc;
using SharedModels;

namespace LoggingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoggingController : ControllerBase
    {
        private static readonly List<ActionLogDto> _logs = new();

        private readonly ILogger<LoggingController> _logger;

        public static List<ActionLogDto> Logs { get => _logs; }

        public LoggingController(ILogger<LoggingController> logger)
        {
            _logger = logger;
        }

        [HttpPost()]
        public IActionResult Log([FromBody] ActionLogDto log) 
        {
            _logs.Add(log);
            return Ok();
        }

        [HttpGet()]
        public IActionResult Get() 
        {
            return Ok(_logs);
        }
    }
}
