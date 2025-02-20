using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraBend.Services.DomainObjects;

namespace UltraBend.Models
{
    [NotifyPropertyChanged]
    public class StudyViewModel
    {
        public Study Study { get; set; } = new Study();

        //public ObservableCollection<Case> Cases { get; set; } = new ObservableCollection<Case>();
    }
}
