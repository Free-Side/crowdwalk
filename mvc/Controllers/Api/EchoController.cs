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
  public class EchoController : Controller {
    private readonly ILogger<EchoController> logger;

    public EchoController(ILogger<EchoController> logger) {
      this.logger = logger;
    }

    [HttpPost]
    public IActionResult Echo([FromBody] Object data) {
      return Ok(data);
    }

    [HttpGet]
    public IActionResult Bad() {
      return BadRequest(new { Message = "not good" });
    }

    [HttpGet]
    public IActionResult Bork() {
      throw new Exception("Double plus un-good.");
    }
  }
}