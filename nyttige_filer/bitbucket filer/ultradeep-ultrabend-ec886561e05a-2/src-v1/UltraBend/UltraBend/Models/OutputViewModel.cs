using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Models
{
    [NotifyPropertyChanged]
    public class OutputViewModel
    {
        public Dictionary<string, Stream> Streams = new Dictionary<string, Stream>();
    }
}
