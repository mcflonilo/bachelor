using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraBend.Common.DomainObjects.Geometry;
using UltraBend.Services.DomainObjects;

namespace UltraBend.Views
{
    public interface IBendStiffenerDesignView : ICoreView
    {
        void Draw(List<DesignSection> sections, DesignSection selectedSection);
        void CenterDiagramOnComputedBounds();
        void UpdateTreeView(List<DesignSection> sections);
        void SetVisible(bool visible);

        event EventHandler<bool> ViewVisibleChanged;
        event EventHandler SizeChanged;
        event EventHandler<List<DesignSection>> SectionsSorted;
        event EventHandler<object> PropertySelected;
    }
}
