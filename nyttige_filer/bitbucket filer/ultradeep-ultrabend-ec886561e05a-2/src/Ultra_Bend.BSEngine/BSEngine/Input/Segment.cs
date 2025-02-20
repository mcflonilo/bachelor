using System.IO;
using System.Threading.Tasks;

namespace UltraBend.Services.BSEngine.Input
{
    public class Segment : IInputDataGroup
    {
        /// <summary>
        ///     Meters
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        ///     Number of elements to use in the segment
        /// </summary>
        public int NumberOfElements { get; set; }

        /// <summary>
        ///     The first outer diameter
        /// </summary>
        public double OuterDiameter1 { get; set; }

        /// <summary>
        ///     The second outer diameter
        /// </summary>
        public double OuterDiameter2 { get; set; }

        /// <summary>
        ///     Density [kg/m^3]
        /// </summary>
        public double Density { get; set; }

        /// <summary>
        ///     Non linear element?
        /// </summary>
        public bool NonLinear { get; set; }

        /// <summary>
        ///     Elastic Modulus [kPa]
        /// </summary>
        public double EModulus { get; set; }

        /// <summary>
        ///     The material name to use when nonlinear
        /// </summary>
        public string MaterialName { get; set; }

        public async Task WriteDataGroupAsync(TextWriter writer)
        {
            if (NonLinear)
            {
                await writer.WriteLineAsync($@"  {Length}     {NumberOfElements}   {OuterDiameter1}   {OuterDiameter2}  {Density}    NOLIN        {MaterialName}  ");
            }
            else
            {
                await writer.WriteLineAsync($@"  {Length}     {NumberOfElements}   {OuterDiameter1}   {OuterDiameter2}  {Density}    LIN        {EModulus:E}  ");
            }
        }
    }
}