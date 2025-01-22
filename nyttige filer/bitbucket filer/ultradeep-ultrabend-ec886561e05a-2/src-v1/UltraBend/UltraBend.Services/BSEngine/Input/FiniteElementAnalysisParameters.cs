using System.IO;
using System.Threading.Tasks;
using UltraBend.Services.Properties;

namespace UltraBend.Services.BSEngine.Input
{
    public class FiniteElementAnalysisParameters : IInputDataGroup
    {
        public FiniteElementAnalysisParameters()
        {
            ToleranceNorm = Settings.Default.ToleranceNorm;
            MaxIterations = Settings.Default.MaxIterations;
            BasisIncrement = Settings.Default.BasisIncrement;
            MinimumIncrement = Settings.Default.MinimumIncrement;
            MaximumIncrement = Settings.Default.MaximumIncrement;
        }
        /// <summary>
        ///     Value of displacement norm, used as convergence criterion for the Newton – Rapshon iteration.
        /// </summary>
        public double ToleranceNorm { get; set; }

        /// <summary>
        ///     Maximum number of iterations
        /// </summary>
        public int MaxIterations { get; set; }

        /// <summary>
        ///     Basis increment (percentage of full load). This should be specified as ‘best estimate’ increment size based on experience.
        /// </summary>
        public double BasisIncrement { get; set; }

        /// <summary>
        ///     Minimum increment (percentage of full load).
        /// </summary>
        public double MinimumIncrement { get; set; }

        /// <summary>
        ///     Maximum increment (percentage of full load).
        /// </summary>
        public double MaximumIncrement { get; set; }

        public async Task WriteDataGroupAsync(TextWriter writer)
        {
            await writer.WriteLineAsync($@"FE ANALYSIS PARAMETERS");
            await writer.WriteLineAsync($@"'");
            await writer.WriteLineAsync($@"'  finite element analysis parameters");
            await writer.WriteLineAsync($@"'");
            await writer.WriteLineAsync($@"'TOLNOR  MAXIT");
            await writer.WriteLineAsync($@" {ToleranceNorm}  {MaxIterations}");
            await writer.WriteLineAsync($@"'DSINC,DSMIN,DSMAX,");
            await writer.WriteLineAsync($@" {BasisIncrement}  {MinimumIncrement} {MaximumIncrement}");
        }
    }
}