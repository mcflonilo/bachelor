using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Views
{
    public interface IProjectView: ICoreView
    {
        void UpdateTreeView(List<Services.DomainObjects.Umbilical> umbilicals, List<Services.DomainObjects.Material> materials, List<Services.DomainObjects.Design> designs, List<Services.DomainObjects.LoadContour> LoadContours, List<Services.DomainObjects.Case> cases, List<Services.DomainObjects.Study> studies);


        event EventHandler CreateNewUmbilical;
        event EventHandler CreateNewDesign;
        event EventHandler CreateNewMaterial;
        event EventHandler CreateNewLoadContour;
        event EventHandler CreateNewCase;
        event EventHandler CreateNewStudy;
        event EventHandler CloseProject;
        event EventHandler<Guid> NodeSelected;
        event EventHandler<object> PropertySelected;
    }
}
