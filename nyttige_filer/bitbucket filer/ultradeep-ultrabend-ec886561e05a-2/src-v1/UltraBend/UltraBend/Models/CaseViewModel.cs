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
    public class CaseViewModel
    {
        public Case Case { get; set; } = new Case();

    }
}
