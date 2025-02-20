using System.IO;
using System.Threading.Tasks;

namespace UltraBend.Services.BSEngine.Input
{
    public class FiniteElementAnalysisParameters : IInputDataGroup
    {
        public FiniteElementAnalysisParameters()
        {

        }

        /// <summary>
        ///     Value of displacement norm, used as convergence criterion for the Newton – Rapshon iteration.
        ///     1E-7
        /// </summary>
        public double ToleranceNorm { get; set; }

        /// <summary>
        ///     Maximum number of iterations
        ///     30
        /// </summary>
        public int MaxIterations { get; set; }

        /// <summary>
        ///     Basis increment (percentage of full load). This should be specified as ‘best estimate’ increment size based on experience.
        ///     0.1
        /// </summary>
        public double BasisIncrement { get; set; }

        /// <summary>
        ///     Minimum increment (percentage of full load).
        ///     0.1
        /// </summary>
        public double MinimumIncrement { get; set; }

        /// <summary>
        ///     Maximum increment (percentage of full load).
        ///     1
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