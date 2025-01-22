using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace UltraBend.Services.BSEngine.Input
{
    public class ElementPrint : IInputDataGroup
    {
        public List<ElementRange> ElementRanges { get; set; }

        public async Task WriteDataGroupAsync(TextWriter writer)
        {
            await writer.WriteLineAsync($@"ELEMENT PRINT");
            await writer.WriteLineAsync($@"'NSPEC");
            await writer.WriteLineAsync($@"{ElementRanges.Count}");
            await writer.WriteLineAsync($@"'IEL1    IEL2");
            foreach (var range in ElementRanges)
                await range.WriteDataGroupAsync(writer);
        }
    }
}