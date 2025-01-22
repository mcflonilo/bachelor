using System.IO;
using System.Threading.Tasks;

namespace UltraBend.Services.BSEngine.Input
{
    public class BendStiffenerConfiguration : IInputDataGroup
    {
        /// <summary>
        ///     Name of this configuration
        /// </summary>
        public string Name { get; set; }

        public BendStiffenerData BendStiffenerData { get; set; }

        public ElementPrint ElementPrint { get; set; }

        public UmbilicalData UmbilicalData { get; set; }

        public FiniteElementAnalysisParameters FiniteElementAnalysisParameters { get; set; }

        public CurvatureRange CurvatureRange { get; set; }

        public Force Force { get; set; }

        public async Task WriteDataGroupAsync(TextWriter writer)
        {
            await writer.WriteLineAsync($@"'---------------------------------------------------------------------");
            await writer.WriteLineAsync($@"' BS-engine");
            await writer.WriteLineAsync($@"'");
            await writer.WriteLineAsync($@"' {Name} - input file");
            await writer.WriteLineAsync($@"'");
            await writer.WriteLineAsync($@"'");

            await writer.WriteLineAsync($@"'---------------------------------------------------------------------");
            await BendStiffenerData.WriteDataGroupAsync(writer);

            await writer.WriteLineAsync($@"'---------------------------------------------------------------------");
            await UmbilicalData.WriteDataGroupAsync(writer);

            await writer.WriteLineAsync($@"'---------------------------------------------------------------------");
            await ElementPrint.WriteDataGroupAsync(writer);

            await writer.WriteLineAsync($@"'---------------------------------------------------------------------");
            await writer.WriteLineAsync($@"FE SYSTEM DATA TEST PRINT");
            await writer.WriteLineAsync($@"'IFSPRI 1/2");
            await writer.WriteLineAsync($@"1");

            await writer.WriteLineAsync($@"'---------------------------------------------------------------------");
            await FiniteElementAnalysisParameters.WriteDataGroupAsync(writer);

            await writer.WriteLineAsync($@"'---------------------------------------------------------------------");
            await CurvatureRange.WriteDataGroupAsync(writer);

            await writer.WriteLineAsync($@"'---------------------------------------------------------------------");
            await Force.WriteDataGroupAsync(writer);

            await writer.WriteLineAsync($@"'---------------------------------------------------------------------");
            await writer.WriteLineAsync($@"EXPORT MATERIAL DATA");
            await writer.WriteLineAsync($@"' IMEX   = 1 : tabular  =2 riflex format");
            await writer.WriteLineAsync($@"1");

            await writer.WriteLineAsync($@"END");
        }
    }
}