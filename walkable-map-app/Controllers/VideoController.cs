using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ServcoHackathon.Data;

namespace ServcoHackathon.WalkableMapApp.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class VideoController : ControllerBase {
        private static readonly IImmutableList<SegmentVideo> Data;

        static VideoController() {
            var fakeDataResource = typeof(VideoController).Assembly.GetManifestResourceStream("ServcoHackathon.WalkableMapApp.Data.Videos.json");
            if (fakeDataResource == null) {
                throw new InvalidOperationException(
                    $"Fake videos data not found. Existing streams: {String.Join(", ", typeof(VideoController).Assembly.GetManifestResourceNames())}"
                );
            }

            using var reader = new StreamReader(fakeDataResource);

            Data = JsonSerializer.Deserialize<SegmentVideo[]>(reader.ReadToEnd(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.ToImmutableList();
        }

        [HttpGet("find")]
        public IActionResult Find(
            [FromQuery] Decimal? top,
            [FromQuery] Decimal? left,
            [FromQuery] Decimal? bottom,
            [FromQuery] Decimal? right) {

            var results = Data.AsQueryable();

            if (top.HasValue) {
                results = results.Where(v => v.Bounds.Bottom < top);
            }
            if (left.HasValue) {
                results = results.Where(v => v.Bounds.Right > left);
            }
            if (bottom.HasValue) {
                results = results.Where(v => v.Bounds.Top > bottom);
            }
            if (right.HasValue) {
                results = results.Where(v => v.Bounds.Left < right);
            }

            // TODO: pagination
            return Ok(results.ToArray());
        }
    }
}
