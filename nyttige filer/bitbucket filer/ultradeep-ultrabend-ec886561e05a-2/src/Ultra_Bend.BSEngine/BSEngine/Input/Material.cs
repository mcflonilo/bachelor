using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace UltraBend.Services.BSEngine.Input
{
    public class Material
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// kPa, [-]
        /// </summary>
        public List<(double Stress, double Strain)> StressStrain { get; set; }
        
        public async Task WriteDataGroupAsync(TextWriter writer)
        {
            await writer.WriteLineAsync($@"'----------------------------------------------------");
            await writer.WriteLineAsync($@"MATERIAL DATA");
            await writer.WriteLineAsync($@"'----------------------------------------------------");
            await writer.WriteLineAsync($@"'{DisplayName}");
            await writer.WriteLineAsync($@"{Name}");
            await writer.WriteLineAsync($@"{StressStrain.Count}");
            foreach (var row in StressStrain)
            {
                await writer.WriteLineAsync($@"{row.Strain}   {row.Stress:E}");
            }
        }
    }
}
