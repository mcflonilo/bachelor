namespace UltraBend.Services.BSEngine.Output
{
    public class BendStiffenerResult
    {
        public int NodeNumber { get; set; }
        public double LengthCoordinate { get; set; }
        public double OuterDiameter { get; set; }
        public double Curvature { get; set; }
        public double Moment { get; set; }
        public double ShearForce { get; set; }
        public double Strain { get; set; }
    }
}