using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Views
{
    public interface IOptimizationView
    {
        void SetVisible(bool visible);

        event EventHandler<bool> ViewVisibleChanged;
        event EventHandler<object> PropertySelected;
    }
}
