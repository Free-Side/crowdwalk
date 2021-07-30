using System;
using Microsoft.AspNetCore.Mvc;

namespace ServcoHackathon.WalkableMapApp.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase {
        public IActionResult Get() {
            return Ok(new {
                Version = typeof(HealthController).Assembly.GetName().Version,
                Time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
    }
}
