using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;
using UltraBend.Common.Math;
using UltraBend.Controls;
using UltraBend.Services.DomainObjects;

namespace UltraBend.Views
{
    public interface IMaterialView: ICoreView
    {
        void Plot(CsvBindingList<MaterialData> data, ExpressionRegression regression, bool allowTemperatureExtrapolation, double temperatureExtrapolationMax, double temperatureExtrapolationMin);
        void SetMaterialDataSource(CsvBindingList<MaterialData> data);
        void SetLinearControlState(bool nonlinear);

        event EventHandler<bool> ViewVisibleChanged;
        event EventHandler SizeChanged;
        event EventHandler DataChanged;
        event GridViewRowEventHandler DefaultValuesNeeded;
        event EventHandler<object> PropertySelected;
    }
}
