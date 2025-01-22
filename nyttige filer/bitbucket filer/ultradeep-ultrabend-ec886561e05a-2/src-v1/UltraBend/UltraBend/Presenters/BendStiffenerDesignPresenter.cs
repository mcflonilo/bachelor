using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI.Docking;
using UltraBend.Common.DomainObjects.Geometry;
using UltraBend.Common.MVP;
using UltraBend.Models;
using UltraBend.Services.DomainObjects;
using UltraBend.Services.Project;
using UltraBend.Views;

namespace UltraBend.Presenters
{
    public class BendStiffenerDesignPresenter : Presenter<IBendStiffenerDesignView, BendStiffenerDesignViewModel>, IPropertySelectable, IButtonGroup, IWindowPresenter
    {
        protected DesignsService designsService { get; set; }

        public bool Active { get; set; } = false;
        
        public string Title
        {
            get { return Model?.Design.Name; }
        }
        
        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return Model?.Design?.Id ?? _id; }
        }

        public DockWindow Window { get; set; }

        public object GetProperty()
        {
            return Model.Design;
        }
        
        public ButtonGroup GetButtonGroup()
        {
            return ButtonGroup.BendStiffenerDesign;
        }

        public ButtonGroup GetButtonGroupToShowByDefault()
        {
            return ButtonGroup.BendStiffenerDesign;
        }

        public bool GetVisible()
        {
            return View.GetVisible();
        }

        public void SetVisible(bool visible)
        {
            Active = visible;
            View.SetVisible(visible);
            
        }

        public BendStiffenerDesignPresenter(IBendStiffenerDesignView view, Guid? id, DesignsService designsService) : base(view)
        {
            this.Model = new BendStiffenerDesignViewModel(){ };

            this.designsService = designsService;

            View.ViewVisibleChanged += (sender, b) => Active = b;
            //View.ViewVisibleChanged += (sender, b) => Bind();
            View.ViewVisibleChanged += (sender, args) => OnVisibleChanged(args);
            View.PropertySelected += (sender, o) => OnPropertySelected(o);
            View.PropertySelected += (sender, o) => Draw();
            View.SizeChanged += (sender, args) => CenterDiagramOnComputedBounds();
            View.SectionsSorted+=ViewOnSectionsSorted;
            
            if (id == null)
            {
                id = Guid.NewGuid();
                designsService.UpsertDesign(new Design() {Id = id.Value, Name = designsService.GetNewName()});
            }

            this.Model.Design = designsService.GetDesignById(id.Value);
            
            Bind();
        }

        public void AddSampleDesign()
        {
            Model.Design.Sections.AddRange(new List<DesignSection>()
            {
                new DesignSection()
                {
                    Id = Guid.NewGuid(),
                    Index = 0,
                    Length = 0.6473,
                    Name = "Stiff Section",
                    RootOuterDiameter = 1,
                    TipOuterDiameter = 1
                },
                new DesignSection()
                {
                    Id = Guid.NewGuid(),
                    Index = 1,
                    Length = 6,
                    Name = "Conical Section",
                    RootOuterDiameter = 1,
                    TipOuterDiameter = 0.325
                },
                new DesignSection()
                {
                    Id = Guid.NewGuid(),
                    Index = 2,
                    Length = .2,
                    Name = "Tip Section",
                    RootOuterDiameter = 0.325,
                    TipOuterDiameter = 0.325
                }
            });
            
            Bind();
        }

        private void ViewOnSectionsSorted(object sender1, List<DesignSection> sections)
        {
            foreach (var sortedSection in sections)
            {
                var section = Model.Design.Sections.FirstOrDefault(s => s.Id == sortedSection.Id);
                if (section != null)
                {
                    section.Index = sortedSection.Index;
                }
            }
            Draw();
        }

        public void AddBendStiffenerSection()
        {
            if (Active)
            {
                Model.Design.Sections.Add(new DesignSection
                {
                    Id = Guid.NewGuid(),
                    Name = "New Section",
                    Index = Model.Design.Sections.Count,
                    Length = Model.Design.Sections.Select(d => d.Length).DefaultIfEmpty(1).LastOrDefault(),
                    RootOuterDiameter = Model.Design.Sections.Select(d => d.TipOuterDiameter).DefaultIfEmpty(1).LastOrDefault(),
                    TipOuterDiameter = Model.Design.Sections.Select(d => d.TipOuterDiameter).DefaultIfEmpty(1).LastOrDefault()
                });
                Bind();
            }
        }

        public void RemoveActiveStiffenerSection()
        {
            if (Active && Model.SelectedSection != null)
            {
                Model.Design.Sections.Remove(Model.SelectedSection);
                Model.SelectedSection = null;
                Bind();
            }
        }

        public void Bind()
        {
            Draw();
            UpdateTreeViewNodes();
            CenterDiagramOnComputedBounds();
            OnPropertySelected(Model.Design);
            SaveChanges();
        }
        private void SaveChanges()
        {
            designsService.UpsertDesign(Model.Design);
        }

        public void Draw()
        {
            View.Draw(Model.Design.Sections, Model.SelectedSection);
        }

        public void UpdateTreeViewNodes()
        {
            View.UpdateTreeView(Model.Design.Sections);
        }

        public void CenterDiagramOnComputedBounds()
        {
            View.CenterDiagramOnComputedBounds();
        }

        public virtual void OnVisibleChanged(bool visible)
        {
            VisibleChanged?.Invoke(this, visible);
        }

        [WeakEvent]
        public event EventHandler<bool> VisibleChanged;

        public void OnPropertySelected(object selection)
        {
            Model.SelectedSection = selection as DesignSection;
            PropertySelected?.Invoke(this, selection);
        }

        [WeakEvent]
        public event EventHandler<object> PropertySelected;
    }
}
