using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace SongSwap_React_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class UserController : ControllerBase
    {
        
    }
}
