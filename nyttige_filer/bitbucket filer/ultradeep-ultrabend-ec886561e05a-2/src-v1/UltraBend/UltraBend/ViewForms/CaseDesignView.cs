using System;
using System.Windows.Forms;
using UltraBend.Models;
using UltraBend.Services.DomainObjects;
using UltraBend.Views;
using PostSharp.Patterns.Model;

namespace UltraBend.ViewForms
{
    public partial class CaseDesignView : UserControl, ICaseView
    {
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

        [WeakEvent]
        public event EventHandler DataChanged;

        public void UpdateViewModel(CaseViewModel model)
        {
            var text = model?.Case?.Result?.Output?.RawOutput;

            // remove previous data
            radTextBox1.Clear();

            // prevent update propagation
            radTextBox1.SuspendUpdate();

            // assign the potentially large amount of text
            radTextBox1.Text = text;

            // resume updates, and clear selections
            radTextBox1.ResumeUpdate();
            radTextBox1.Select(0,0);

        }

        public void UpdateReport()
        {
            //reportViewer1.RefreshReport();
        }

        public CaseDesignView()
        {
            Dock = DockStyle.Fill;
            VisibleChanged += (sender, args) => OnViewVisibleChanged(this.Visible);

            InitializeComponent();
        }
        public void OnViewVisibleChanged(bool visible)
        {
            ViewVisibleChanged?.Invoke(this, visible);
        }
        
        public void OnPropertySelected(Case selection)
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

        private void CaseDesignView_Load(object sender, EventArgs e)
        {
            //reportViewer1.RefreshReport();   

            OnViewLoading();
        }

        public bool GetVisible()
        {
            throw new NotImplementedException();
        }
    }
}
