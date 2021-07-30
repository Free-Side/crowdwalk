using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using ServcoHackathon.Data;

namespace ServcoHackathon.WalkableMapApp.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ActivityController : ControllerBase {
        [HttpPost("convert-to-json")]
        public IActionResult ConvertToJson([FromBody] XDocument activity) {
            if (activity.Root == null) {
                return BadRequest(new ProblemDetails {
                    Detail = "No GPX Data Received."
                });
            }

            if (String.Equals(activity.Root.Name.NamespaceName, GpxData.Namespace)) {
                var gpxSerializer = new XmlSerializer(typeof(GpxData));
                using var reader = activity.Root.CreateReader();
                var route = (GpxData)gpxSerializer.Deserialize(reader);

                if (route == null) {
                    return BadRequest(new ProblemDetails {
                        Detail = "No GPX Data Received."
                    });
                }

                var points =
                    route.Track.Segments
                        .SelectMany(seg => seg.Points.Select(
                            point => new {
                                point.Latitude,
                                point.Longitude,
                                point.Time
                            }
                        ))
                        .ToList();

                var bounds =
                    points.Aggregate(
                        new {
                            Top = (Decimal?)null,
                            Left = (Decimal?)null,
                            Bottom = (Decimal?)null,
                            Right = (Decimal?)null,
                            Latitude = 0m,
                            Longitude = 0m,
                            Count = 0
                        },
                        (avg, point) => new {
                            // Note: in the event that an activity spans the 180/-180 line of longitude this boundary computation will be junk.
                            // TODO: come up with a boundary computation that accounts for this
                            Top = !avg.Top.HasValue || point.Latitude > avg.Top.Value ? point.Latitude : avg.Top,
                            Left = !avg.Left.HasValue || point.Longitude < avg.Left.Value ? point.Longitude : avg.Left,
                            Bottom = !avg.Bottom.HasValue || point.Latitude < avg.Bottom.Value ? point.Latitude : avg.Bottom,
                            Right = !avg.Right.HasValue || point.Longitude > avg.Right.Value ? point.Longitude : avg.Right,
                            Latitude = avg.Count > 0 ? (avg.Latitude + point.Latitude / avg.Count) * ((Decimal)avg.Count / (avg.Count + 1)) : point.Latitude,
                            Longitude =
                                avg.Count > 0 ? (avg.Longitude + point.Longitude / avg.Count) * ((Decimal)avg.Count / (avg.Count + 1)) : point.Longitude,
                            Count = avg.Count + 1
                        }
                    );

                return Ok(new {
                    Points = points,
                    Center = new { Latitude = bounds.Latitude, Longitude = bounds.Longitude },
                    Bounds = new {
                        Top = bounds.Top,
                        Left = bounds.Left,
                        Bottom = bounds.Bottom,
                        Right = bounds.Right,
                    }
                });
            } else if (String.Equals(activity.Root.Name.NamespaceName, TcxData.Namespace)) {
                var tcxSerializer = new XmlSerializer(typeof(TcxData));
                using var reader = activity.Root.CreateReader();
                var route = (TcxData)tcxSerializer.Deserialize(reader);

                if (route == null) {
                    return BadRequest(new ProblemDetails {
                        Detail = "No GPX Data Received."
                    });
                }

                var points =
                    route.Courses
                        .SelectMany(course => course.Track.Select(
                            point => new {
                                point.Position.Latitude,
                                point.Position.Longitude,
                                point.Time
                            }
                        ))
                        .ToList();

                var bounds =
                    points.Aggregate(
                        new {
                            Top = (Decimal?)null,
                            Left = (Decimal?)null,
                            Bottom = (Decimal?)null,
                            Right = (Decimal?)null,
                            Latitude = 0m,
                            Longitude = 0m,
                            Count = 0
                        },
                        (avg, point) => new {
                            // Note: in the event that an activity spans the 180/-180 line of longitude this boundary computation will be junk.
                            // TODO: come up with a boundary computation that accounts for this
                            Top = !avg.Top.HasValue || point.Latitude > avg.Top.Value ? point.Latitude : avg.Top,
                            Left = !avg.Left.HasValue || point.Longitude < avg.Left.Value ? point.Longitude : avg.Left,
                            Bottom = !avg.Bottom.HasValue || point.Latitude < avg.Bottom.Value ? point.Latitude : avg.Bottom,
                            Right = !avg.Right.HasValue || point.Longitude > avg.Right.Value ? point.Longitude : avg.Right,
                            Latitude = avg.Count > 0 ? (avg.Latitude + point.Latitude / avg.Count) * ((Decimal)avg.Count / (avg.Count + 1)) : point.Latitude,
                            Longitude =
                                avg.Count > 0 ? (avg.Longitude + point.Longitude / avg.Count) * ((Decimal)avg.Count / (avg.Count + 1)) : point.Longitude,
                            Count = avg.Count + 1
                        }
                    );

                return Ok(new {
                    Points = points,
                    Center = new { Latitude = bounds.Latitude, Longitude = bounds.Longitude },
                    Bounds = new {
                        Top = bounds.Top,
                        Left = bounds.Left,
                        Bottom = bounds.Bottom,
                        Right = bounds.Right,
                    }
                });
            } else {
                return BadRequest(new ProblemDetails {
                    Detail = "The XML posted did not have a valid Namespace. Only valid GPX and TCX data is supported."
                });
            }
        }
    }
}
