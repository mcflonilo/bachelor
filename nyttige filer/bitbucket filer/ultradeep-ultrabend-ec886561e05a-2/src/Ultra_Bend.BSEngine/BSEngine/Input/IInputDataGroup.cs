using System.IO;
using System.Threading.Tasks;

namespace UltraBend.Services.BSEngine.Input
{
    public interface IInputDataGroup
    {
        Task WriteDataGroupAsync(TextWriter writer);
    }
}