using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraBend.Common.MVP;

namespace UltraBend.Views
{
    public interface ICoreView : IView
    {
        void SetVisible(bool visible);
        bool GetVisible();

        event EventHandler ViewLoading;
    }
}
