using System;

namespace ServcoHackathon.Data {
    public class SegmentVideo {
        public String Title { get; set; }
        public Uri Url { get; set; }
        public Point[] Points { get; set; }
        public Point Center { get; set; }
        public Bounds Bounds { get; set; }
    }
}
