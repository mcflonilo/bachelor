using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.Reporting;
using UltraBend.Models;
using UltraBend.Reports.DataSources;
using UltraBend.Services.DomainObjects;
using UltraBend.Views;
using Telerik.WinControls.UI;
using UltraBend.Services.Project;
using UltraBend.Common.Units;
using PostSharp.Patterns.Model;

namespace UltraBend.ViewForms
{
    public partial class StudyView : UserControl, IStudyView
    {
        [WeakEvent]
        public event EventHandler DataChanged;

        [WeakEvent]
        public event EventHandler ViewLoading;
        public void SetVisible(bool visible)
        {
            this.Visible = visible;
            OnViewVisibleChanged(visible);
        }

        [WeakEvent]
        public event EventHandler<object> PropertySelected;

        [WeakEvent]
        public event EventHandler<bool> ViewVisibleChanged;

        private StudyViewModel Model { get; set; }

        public void UpdateViewModel(StudyViewModel model)
        {
            Model = model;
            radGridViewStudy.BindingContext = new System.Windows.Forms.BindingContext();
            radGridViewStudy.DataSource = model?.Study?.Cases;
        }


        public StudyView()
        {
            Dock = DockStyle.Fill;
            VisibleChanged += (sender, args) => OnViewVisibleChanged(this.Visible);

            InitializeComponent();

            radGridViewStudy.Columns["Temperature"].DataTypeConverter = new DimensionTypeConverter<TemperatureDimension>();
            radGridViewStudy.Columns["UmbilicalTension"].DataTypeConverter = new DimensionTypeConverter<ForceDimension>();
            radGridViewStudy.Columns["DeflectionAngle"].DataTypeConverter = new DimensionTypeConverter<AngleDimension>();

        }


        public void OnViewVisibleChanged(bool visible)
        {
            ViewVisibleChanged?.Invoke(this, visible);
        }
        
        public void OnPropertySelected(Study selection)
        {
            PropertySelected?.Invoke(this, selection);
        }
        protected virtual void OnViewLoading()
        {
            ViewLoading?.Invoke(this, EventArgs.Empty);
        }

        public DialogResult ShowDialog()
        {
            throw new NotImplementedException();
        }

        private void StudyDesignView_Load(object sender, EventArgs e)
        {
            //reportViewer1.RefreshReport();   

            OnViewLoading();
        }

        public bool GetVisible()
        {
            return this.Visible;
        }

        private void radGridViewStudy_EditorRequired(object sender, Telerik.WinControls.UI.EditorRequiredEventArgs e)
        {
            if (this.radGridViewStudy.CurrentColumn.FieldName == "Design")
            {
                var designsService = new DesignsService(ApplicationState.CurrentProjectTemporaryFile);
                var designs = designsService.GetDesigns();

                var editor = new DesignRadDropDownListEditor();
                var element = (RadDropDownListEditorElement)editor.EditorElement;
                element.DataSource = designs;
                element.DisplayMember = "Name";
                e.Editor = editor;
            }

            if (this.radGridViewStudy.CurrentColumn.FieldName == "Temperature")
            {

            }
        }

        public class DesignRadDropDownListEditor : RadDropDownListEditorElement
        {
            public override object Value
            {
                get
                {
                    return ((Design)this.SelectedValue);
                }
                set
                {
                    this.SelectedValue = value;
                }
            }
        }

        private void radGridViewStudy_CellBeginEdit(object sender, Telerik.WinControls.UI.GridViewCellCancelEventArgs e)
        {

        }

        private void radGridViewStudy_DefaultValuesNeeded(object sender, GridViewRowEventArgs e)
        {
            if (this.radGridViewStudy.CurrentRow is GridViewNewRowInfo)
            {
                e.Row.Cells["Temperature"].Value = 273.0;
                e.Row.Cells["UmbilicalTension"].Value = 1000.0;
                e.Row.Cells["DeflectionAngle"].Value = 0.0;
                e.Row.Cells["StudyId"].Value = Model.Study.Id;
                //e.Row.Cells["Id"].Value = Guid.NewGuid();

                //var designsService = new DesignsService(ApplicationState.CurrentProjectTemporaryFile);
                //var designs = designsService.GetDesigns();

                //if (designs.Any())
                //{
                //    e.Row.Cells["Design"].Value = designs.First();
                //}
            }
        }

        private void radGridViewStudy_CellEndEdit(object sender, GridViewCellEventArgs e)
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }

        private void caseBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void radGridViewStudy_RowsChanged(object sender, GridViewCollectionChangedEventArgs e)
        {
            DataChanged?.Invoke(this, EventArgs.Empty);

        }
    }
}
