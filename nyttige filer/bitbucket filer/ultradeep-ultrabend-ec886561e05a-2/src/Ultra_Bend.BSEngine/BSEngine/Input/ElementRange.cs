using System.IO;
using System.Threading.Tasks;

namespace UltraBend.Services.BSEngine.Input
{
    public class ElementRange : IInputDataGroup
    {
        /// <summary>
        ///     Starting element index
        /// </summary>
        public int StartElementIndex { get; set; }

        /// <summary>
        ///     Ending element index
        /// </summary>
        public int EndElementIndex { get; set; }

        public async Task WriteDataGroupAsync(TextWriter writer)
        {
            await writer.WriteLineAsync($@"{StartElementIndex}         {EndElementIndex}");
        }
    }
}