using System.IO;
using System.Threading.Tasks;
using UltraBend.Services.Properties;

namespace UltraBend.Services.BSEngine.Input
{
    public class CurvatureRange : IInputDataGroup
    {
        public CurvatureRange()
        {
            MaximumCurvature = Settings.Default.MaximumCurvature;
            Number = Settings.Default.Number;
        }
        /// <summary>
        ///     Maximum curvature to for tabulation of moment-curvature.
        ///     Moment-curvature will by this specification be tabulated at NCURV equidistant
        ///     points in the interval 0 .≤ curvature.≤ CURMAX
        ///     CURMAX should represent a realistic upper bound value for the expected
        ///     maximum curvature response of the bend stiffener
        ///     [1/m]
        /// </summary>
        public double MaximumCurvature { get; set; }

        /// <summary>
        ///     Number of points in moment-curvature table.
        ///     NCURV ≥ 2
        ///     NCURV should be selected to represent the nonlinear strain-stress curve of
        ///     the material accurately, typically NCURV ≥ NMAT
        /// </summary>
        public int Number { get; set; }

        public async Task WriteDataGroupAsync(TextWriter writer)
        {
            await writer.WriteLineAsync($@"CURVATURE RANGE");
            await writer.WriteLineAsync($@"'CURMAX  - Maximum curvature ");
            await writer.WriteLineAsync($@"'NCURV   - Number of curvature levels");
            await writer.WriteLineAsync($@"'");
            await writer.WriteLineAsync($@"'CURMAX (1/m),NCURV");
            await writer.WriteLineAsync($@"{MaximumCurvature}       {Number}");
        }
    }
}