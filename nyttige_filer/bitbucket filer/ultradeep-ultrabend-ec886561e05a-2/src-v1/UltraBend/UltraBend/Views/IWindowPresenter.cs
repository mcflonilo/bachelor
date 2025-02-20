using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI.Docking;

namespace UltraBend.Views
{
    public interface IWindowPresenter
    {
        string Title { get; }

        Guid Id { get; }

        DockWindow Window { get; set; }

        void SetVisible(bool visible);
        bool GetVisible();
    }
}
