using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UltraBend.Views;
using UltraBend.Services.DomainObjects;
using UltraBend.Models;
using UltraBend.Common.Units;
using Telerik.WinControls.UI;
using PostSharp.Patterns.Model;

namespace UltraBend.ViewForms
{
    public partial class UmbilicalView : UserControl, IUmbilicalView
    {
        public UmbilicalView()
        {
            Dock = DockStyle.Fill;
            VisibleChanged += (sender, args) => OnViewVisibleChanged(this.Visible);
            InitializeComponent();
            radDataEntry1.BindingCreated += RadDataEntry1_BindingCreated;
        }

        private void RadDataEntry1_BindingCreated(object sender, Telerik.WinControls.UI.BindingCreatedEventArgs e)
        {
            if (e.DataMember == "Id")
            {
                (e.Control as RadTextBox).ReadOnly = true;
            }
            if (e.DataMember == "Length")
            {
                e.Binding.FormattingEnabled = true;
                e.Binding.Parse += Binding_Parse_Length;
                e.Binding.Format += Binding_Format_Length;
            }
            if (e.DataMember == "BendingStiffness")
            {
                e.Binding.FormattingEnabled = true;
                e.Binding.Parse += Binding_Parse_Torsion;
                e.Binding.Format += Binding_Format_Torsion;
            }
            if (e.DataMember == "AxialStiffness")
            {
                e.Binding.FormattingEnabled = true;
                e.Binding.Parse += Binding_Parse_Force;
                e.Binding.Format += Binding_Format_Force;
            }
            if (e.DataMember == "TorsionalStiffness")
            {
                e.Binding.FormattingEnabled = true;
                e.Binding.Parse += Binding_Parse_Torsion;
                e.Binding.Format += Binding_Format_Torsion;
            }
            if (e.DataMember == "Mass")
            {
                e.Binding.FormattingEnabled = true;
                e.Binding.Parse += Binding_Parse_Mass;
                e.Binding.Format += Binding_Format_Mass;
            }
            if (e.DataMember == "Diameter")
            {
                e.Binding.FormattingEnabled = true;
                e.Binding.Parse += Binding_Parse_Length;
                e.Binding.Format += Binding_Format_Length;
            }
        }

        private void Binding_Parse_Length(object sender, ConvertEventArgs e)
        {
            e.Value = new DimensionTypeConverter<LengthDimension>().ConvertTo((string)e.Value, typeof(double));
        }

        private void Binding_Format_Length(object sender, ConvertEventArgs e)
        {
            e.Value = new DimensionTypeConverter<LengthDimension>().ConvertTo((double)e.Value, typeof(string));
        }

        private void Binding_Parse_Torsion(object sender, ConvertEventArgs e)
        {
            e.Value = new DimensionTypeConverter<TorsionalStiffnessDimension>().ConvertTo((string)e.Value, typeof(double));
        }

        private void Binding_Format_Torsion(object sender, ConvertEventArgs e)
        {
            e.Value = new DimensionTypeConverter<TorsionalStiffnessDimension>().ConvertTo((double)e.Value, typeof(string));
        }

        private void Binding_Parse_Force(object sender, ConvertEventArgs e)
        {
            e.Value = new DimensionTypeConverter<ForceDimension>().ConvertTo((string)e.Value, typeof(double));
        }

        private void Binding_Format_Force(object sender, ConvertEventArgs e)
        {
            e.Value = new DimensionTypeConverter<ForceDimension>().ConvertTo((double)e.Value, typeof(string));
        }

        private void Binding_Parse_Mass(object sender, ConvertEventArgs e)
        {
            e.Value = new DimensionTypeConverter<MassPerLengthStiffnessDimension>().ConvertTo((string)e.Value, typeof(double));
        }

        private void Binding_Format_Mass(object sender, ConvertEventArgs e)
        {
            e.Value = new DimensionTypeConverter<MassPerLengthStiffnessDimension>().ConvertTo((double)e.Value, typeof(string));
        }

        [WeakEvent]
        public event EventHandler<bool> ViewVisibleChanged;

        [WeakEvent]
        public event EventHandler DataChanged;

        [WeakEvent]
        public event EventHandler<object> PropertySelected;

        [WeakEvent]
        public event EventHandler ViewLoading;

        public bool GetVisible()
        {
            return this.Visible;
        }

        public void SetUmbilicalDataSource(Umbilical model)
        {
            radDataEntry1.DataSource = model;
        }

        public void SetVisible(bool visible)
        {
            this.Visible = visible;
            OnViewVisibleChanged(visible);
        }
        public void OnViewVisibleChanged(bool visible)
        {
            ViewVisibleChanged?.Invoke(this, visible);
        }
        protected virtual void OnViewLoading()
        {
            ViewLoading?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
            PropertySelected?.Invoke(this, radDataEntry1.DataSource);
        }

        public DialogResult ShowDialog()
        {
            throw new NotImplementedException();
        }

        private void UmbilicalView_Load(object sender, EventArgs e)
        {
            OnViewLoading();
        }

        public void UpdateViewModel(UmbilicalViewModel model)
        {
            radDataEntry1.DataSource = model?.Umbilical;
        }

        private void radDataEntry1_ItemValidated(object sender, Telerik.WinControls.UI.ItemValidatedEventArgs e)
        {
            OnDataChanged();
        }
    }
}
