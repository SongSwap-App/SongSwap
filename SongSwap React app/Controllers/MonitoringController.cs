using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SongSwap_React_app.Infrastructure;

namespace SongSwap_React_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitoringController : ControllerBase
    {
        public static List<Tuple<DateTime, string,double>> logs = new List<Tuple<DateTime, string, double>>();

        [HttpGet] 
        public IActionResult Logs() 
        {
            return Ok(logs);
        }

        [HttpGet("heavy")]
        public IActionResult HeavyRequests() 
        {
            return Ok(logs.OrderByDescending(l => l.Item3));
        }
    }
}
