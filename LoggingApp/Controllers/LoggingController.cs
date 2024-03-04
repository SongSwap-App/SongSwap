using LoggingApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace LoggingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoggingController : ControllerBase
    {
        private readonly List<ActionLog> _logs;

        private readonly ILogger<LoggingController> _logger;

        public LoggingController(ILogger<LoggingController> logger)
        {
            _logs = new List<ActionLog>();
            _logger = logger;
        }

        [HttpGet("hello")]
        public IActionResult Hello()
        {
            return Ok("Hello world!");
        }

        [HttpPost()]
        public IActionResult Log(ActionLog log) 
        {
            return Ok(log);
        }

        [HttpGet()]
        public IActionResult Get() 
        {
            return Ok(_logs);
        }
    }
}
