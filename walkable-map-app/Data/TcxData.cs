using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServcoHackathon.Data {
    [XmlRoot("TrainingCenterDatabase", Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public class TcxData {
        public const String Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2";

        [XmlArray(ElementName = "Courses", Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
        public List<Course> Courses { get; set; }
    }

    public class Course {
        [XmlElement("Name", Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
        public String Name { get; set; }

        [XmlArrayItem("Trackpoint", IsNullable = false, Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
        [XmlArray("Track", Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
        public List<CourseTrackPoint> Track { get; set; }
    }

    public class CourseTrackPoint {
        [XmlElement("Time", Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
        public DateTime Time { get; set; }

        [XmlElement("Position", Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
        public Position Position { get; set; }

        [XmlElement("AltitudeMeters", Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
        public Decimal Altitude { get; set; }

        [XmlElement("DistanceMeters", Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
        public Decimal Distance { get; set; }
    }

    public class Position {
        [XmlElement("LatitudeDegrees", Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
        public Decimal Latitude { get; set; }

        [XmlElement("LongitudeDegrees", Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
        public Decimal Longitude { get; set; }
    }
}
