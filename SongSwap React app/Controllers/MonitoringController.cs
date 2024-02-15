using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SongSwap_React_app.Infrastructure;

namespace SongSwap_React_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitoringController : ControllerBase
    {
        private static readonly List<ActionLog> logs = new();

        public static List<ActionLog> Logs { get => logs; }

        [HttpGet] 
        public IActionResult GetLogs() 
        {
            return Ok(logs);
        }

        [HttpGet("heavy")]
        public IActionResult HeavyRequests() 
        {
            return Ok(logs.OrderByDescending(l => l.TimeElapsed));
        }
    }
}
