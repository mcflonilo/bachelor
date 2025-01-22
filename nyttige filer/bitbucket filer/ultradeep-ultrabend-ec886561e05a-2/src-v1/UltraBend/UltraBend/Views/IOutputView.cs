using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Views
{
    public interface IOutputView: ICoreView
    {
        //List<Stream> GetChannelStreams(List<string> channels);
        Stream AddChannel(string channel);
        void RemoveChannel(string channel);
    }
}
