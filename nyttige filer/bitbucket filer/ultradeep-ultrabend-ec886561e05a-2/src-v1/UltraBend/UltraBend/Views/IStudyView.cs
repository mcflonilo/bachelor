using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraBend.Models;

namespace UltraBend.Views
{
    public interface IStudyView : ICoreView
    {

        void SetVisible(bool visible);

        event EventHandler<object> PropertySelected;
        event EventHandler<bool> ViewVisibleChanged;
        event EventHandler DataChanged;

        void UpdateViewModel(StudyViewModel model);
    }
}
