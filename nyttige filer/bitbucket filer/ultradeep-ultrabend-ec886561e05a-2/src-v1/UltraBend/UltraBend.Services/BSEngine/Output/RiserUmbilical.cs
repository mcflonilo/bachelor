namespace UltraBend.Services.BSEngine.Output
{
    public class RiserUmbilical
    {
        public int NumberOfElements { get; set; }
        public int FirstElementNumber { get; set; }
        public int LastElementNumber { get; set; }
        public double Length { get; set; }
        public double ElementLength { get; set; }
        public double BendStiffness { get; set; }
        public double AxialStiffness { get; set; }
        public double TorsionalStiffness { get; set; }
        public double MassPerUnitLength { get; set; }
    }
}