namespace UltraBend.Services.BSEngine.Output
{
    public class KeyResults
    {
        public double BSMomentAtRootEnd { get; set; }
        public double BSShearForceAtRootEnd { get; set; }
        public double RiserTensionAtRootEnd { get; set; }
        public double MaximumBSCurvature { get; set; }
        public double MaximumBSStrainAtOuterDiameter { get; set; }
        public double MaximumCurvature { get; set; }
        public double ZDisplacementOfRiserTip { get; set; }
    }
}