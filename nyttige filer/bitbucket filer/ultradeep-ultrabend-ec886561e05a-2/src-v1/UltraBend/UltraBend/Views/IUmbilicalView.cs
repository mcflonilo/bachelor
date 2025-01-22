using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraBend.Models;

namespace UltraBend.Views
{
    public interface IUmbilicalView : ICoreView
    {
        void SetUmbilicalDataSource(UltraBend.Services.DomainObjects.Umbilical model);
        void UpdateViewModel(UmbilicalViewModel model);


        event EventHandler<bool> ViewVisibleChanged;
        event EventHandler DataChanged;
        event EventHandler<object> PropertySelected;
    }
}
