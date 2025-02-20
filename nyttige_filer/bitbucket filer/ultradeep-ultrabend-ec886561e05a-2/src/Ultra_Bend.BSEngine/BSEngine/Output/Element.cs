namespace UltraBend.Services.BSEngine.Output
{
    public class Element
    {
        public int ElementNumber { get; set; }
        public double LengthCoordinate { get; set; } = double.NaN;
        public double OuterDiameter { get; set; } = double.NaN;
        public double Length { get; set; } = double.NaN;
        public double AxialStiffness { get; set; } = double.NaN;
        public double BendingStiffnessY { get; set; } = double.NaN;
        public double BendingStiffnessZ { get; set; } = double.NaN;
        public double TorsionalStiffness { get; set; } = double.NaN;
        public double Weight { get; set; } = double.NaN;
        public double Buoyancy { get; set; } = double.NaN;
    }
}