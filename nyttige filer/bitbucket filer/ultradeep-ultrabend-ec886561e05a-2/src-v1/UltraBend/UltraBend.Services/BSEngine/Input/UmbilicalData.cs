using System.IO;
using System.Threading.Tasks;

namespace UltraBend.Services.BSEngine.Input
{
    public class UmbilicalData : IInputDataGroup
    {
        /// <summary>
        ///     Length of riser (from bend stiffener tip to free end)  [m]
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        ///     Number of elements.  A constant element mesh is applied
        /// </summary>
        public int NumberOfElements { get; set; }

        /// <summary>
        ///     Bending stiffness. [kN]
        /// </summary>
        public double BendingStiffness { get; set; }

        /// <summary>
        ///     Axial stiffness. [kN]
        /// </summary>
        public double AxialStiffness { get; set; }

        /// <summary>
        ///     Torsional stiffness. [kN m^2]
        /// </summary>
        public double TorsionalStiffness { get; set; }

        /// <summary>
        ///     Mass per unit length [kg/m]
        /// </summary>
        public double Mass { get; set; }

        public async Task WriteDataGroupAsync(TextWriter writer)
        {
            await writer.WriteLineAsync($@"RISER DATA");
            await writer.WriteLineAsync($@"'SRIS,  NEL  EI,      EA,    GT        Mass  ");
            await writer.WriteLineAsync($@"' (m)   (-)  (kNm^2)  (kN)   (kNm^2))  (kg/m)");
            await writer.WriteLineAsync(
                $@" {Length}    {NumberOfElements}    {BendingStiffness}   {AxialStiffness}  {TorsionalStiffness}      {
                        Mass
                    }  ");
        }
    }
}