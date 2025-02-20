using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace UltraBend.Services.BSEngine.Input
{
    public class BendStiffenerData : IInputDataGroup
    {
        /// <summary>
        ///     Segments of the bend stiffener
        /// </summary>
        public List<Segment> Segments { get; set; } = new List<Segment>();

        /// <summary>
        ///     All materials used
        /// </summary>
        public List<Material> Materials { get; set; } = new List<Material>();

        /// <summary>
        ///     Inner diameter [m]
        /// </summary>
        public double InnerDiameter { get; set; }

        public async Task WriteDataGroupAsync(TextWriter writer)
        {
            await writer.WriteLineAsync($@"BEND STIFFENER DATA");
            await writer.WriteLineAsync($@"' ID                  NSEG");
            await writer.WriteLineAsync($@"' inner diameter      Number of linear segments");
            await writer.WriteLineAsync($@"  {InnerDiameter}                 {Segments.Count}");
            await writer.WriteLineAsync($@"' LENGTH   NEL   OD1     OD2    DENSITY  LIN/NOLIN  EMOD/MAT-ID");
            await writer.WriteLineAsync($@"' (m)      (-)   (m)     (m)    (kg/m3)  (-)        (kPa)");
            foreach (var segment in Segments)
                await segment.WriteDataGroupAsync(writer);
            foreach (var material in Materials)
                await material.WriteDataGroupAsync(writer);
        }
    }
}