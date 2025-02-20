using System.IO;
using System.Threading.Tasks;

namespace UltraBend.Services.BSEngine.Input
{
    public class Force : IInputDataGroup
    {
        /// <summary>
        ///     Force direction (deg) relative to stress-free configuration. (positive y-rotation
        ///     angle, i.e.positive in clock-wise direction)
        /// </summary>
        public double ForceDirection { get; set; }

        /// <summary>
        ///     Effective tension [kN]
        /// </summary>
        public double Tension { get; set; }

        public async Task WriteDataGroupAsync(TextWriter writer)
        {
            await writer.WriteLineAsync($@"FORCE");
            await writer.WriteLineAsync($@"'Relang  tension");
            await writer.WriteLineAsync($@"'(deg)   (kN)");
            await writer.WriteLineAsync($@"{ForceDirection:F2}   {Tension:F1}");
        }
    }
}