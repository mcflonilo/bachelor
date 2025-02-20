using System.Collections.Generic;

namespace UltraBend.Services.BSEngine.Output
{
    public class Segment
    {
        public int SegmentNumber { get; set; }
        public int NumberOfElements { get; set; }
        public int FirstElementNumber { get; set; }
        public int LastElementNumber { get; set; }
        public double SegmentLength { get; set; }
        public double ElementLength { get; set; }
        public double OuterDiameterEnd1 { get; set; }
        public double OuterDiameterEnd2 { get; set; }
        public double Density { get; set; }
        public string MaterialModel { get; set; }
        public double EModulus { get; set; }
        public List<Element> Elements { get; set; } = new List<Element>();
    }
}