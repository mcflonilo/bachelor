using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI.Docking;
using UltraBend.Common.MVP;
using UltraBend.Models;
using UltraBend.Services.Project;
using UltraBend.Views;

namespace UltraBend.Presenters
{
    public class UmbilicalPresenter : Presenter<IUmbilicalView, UmbilicalViewModel>, IPropertySelectable, IButtonGroup, IWindowPresenter
    {
        protected UmbilicalsService umbilicalsService { get; set; }

        public bool Active { get; set; } = false;

        public UmbilicalPresenter(IUmbilicalView view, Guid? id, UmbilicalsService umbilicalsSErvice) : base(view)
        {
            this.umbilicalsService = umbilicalsSErvice;

            this.Model = new UmbilicalViewModel() { Umbilical = null };
            if (id == null)
            {
                id = Guid.NewGuid();
                var newModel = new Services.DomainObjects.Umbilical() { Id = id.Value, Name = umbilicalsService.GetNewName() };
                umbilicalsService.UpsertUmbilical(newModel);
            }
            this.Model.Umbilical = umbilicalsService.GetUmbilicalById(id.Value);

            View.ViewLoading += (sender, args) => Bind();
            View.DataChanged += (sender, args) => SaveChanges();
            View.PropertySelected += (sender, args) => PropertySelected?.Invoke(sender, args);

            View.UpdateViewModel(this.Model);
        }

        public string Title { get { return Model?.Umbilical.Name; } }

        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return Model?.Umbilical?.Id ?? _id; }
        }

        public DockWindow Window { get; set; }

        public ButtonGroup GetButtonGroup()
        {
            return ButtonGroup.BendStiffenerDesign;
        }

        public ButtonGroup GetButtonGroupToShowByDefault()
        {
            return ButtonGroup.BendStiffenerDesign;
        }
        public object GetProperty()
        {
            return Model.Umbilical;
        }

        [WeakEvent]
        public event EventHandler<object> PropertySelected;
        public virtual void OnPropertySelected(object selection)
        {
            PropertySelected?.Invoke(this, selection);
        }
        public void Bind()
        {

            SaveChanges();
        }

        public bool GetVisible()
        {
            return View.GetVisible();
        }

        public void SetVisible(bool visible)
        {
            Active = visible;
            View.SetVisible(visible);
            OnPropertySelected(Model.Umbilical);
        }
        private void SaveChanges()
        {
            umbilicalsService.UpsertUmbilical(Model.Umbilical);
        }
    }
}
