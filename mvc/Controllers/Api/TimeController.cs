using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SampleWebAPI.Models;

namespace SampleWebAPI.Controllers {
  [Route("api/[controller]")]
  public class TimeController : Controller {
    private readonly ILogger<TimeController> logger;

    public TimeController(ILogger<TimeController> logger) {
      this.logger = logger;
    }

    [HttpGet]
    public IActionResult Get() {
      var time = DateTime.UtcNow;
      var localTime = DateTime.Now;
      var localZone = TimeZoneInfo.Local;
      this.logger.LogInformation("Reporting the current time {UtcTime}", time);
      return Ok(new {
        CurrentTime = time,
        ServerTimeZone =
          localZone.IsDaylightSavingTime(localTime) ?
            localZone.DaylightName :
            localZone.StandardName
      });
    }
  }
}