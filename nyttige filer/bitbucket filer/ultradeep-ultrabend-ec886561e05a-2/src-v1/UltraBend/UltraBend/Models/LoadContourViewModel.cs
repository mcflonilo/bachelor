using System.Collections.Generic;
using PostSharp.Patterns.Model;

namespace UltraBend.Models
{
    [NotifyPropertyChanged]
    public class LoadContourViewModel
    {
        public Services.DomainObjects.LoadContour LoadContour { get; set; } = new Services.DomainObjects.LoadContour();

        public List<LoadContourViewModelRowEntry> Rows { get; set; }

    }
}