using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraBend.Services.DomainObjects;

namespace UltraBend.Models
{
    [NotifyPropertyChanged]
    public class BendStiffenerDesignViewModel
    {
        public Design Design { get; set; } = new Design();

        public DesignSection SelectedSection { get; set; } = null;
    }
}
