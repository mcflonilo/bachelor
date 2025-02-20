using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI.Docking;

namespace UltraBend.Models
{
    [NotifyPropertyChanged]
    public class MainViewModel
    {
        public string ProjectName { get; set; }

        public string ProjectFileName { get; set; }

        public List<DocumentWindow> DocumentWindows { get; set; } = new List<DocumentWindow>();
    }
}
