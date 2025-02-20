using PostSharp.Patterns.Model;
using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;

namespace UltraBend.Views
{
    public interface IMainView : ICoreView
    {
        string Title { get; set; }
        void ConfirmLoaded();
        void PropertySelect(object selectedObject);
        void UpdateButtonGroup(ButtonGroup buttonGroup); //, bool active);
        void AddDocumentWindow(DocumentWindow window);
        bool HasDocumentWindow(DockWindow window);

        void SetOutputControl(UserControl control);
        void ClearOutputControl();

        void SetProjectControl(UserControl projectControl);
        void ClearProjectControl();

        void RestoreWindow(Point point, Size size, bool maximized);
        Tuple<Point, Size, bool> GetWindowState();
        void SetRecentProjects(StringCollection projectFiles);


        event EventHandler<string> PropertyGridValueChanged;
        event EventHandler AddBendStiffenerSection;
        event EventHandler RemoveActiveStiffenerSection;
        event EventHandler CreateNewDesign;
        event EventHandler CreateNewMaterial;
        event EventHandler CreateNewLoadContour;
        event EventHandler CreateNewCase;
        event EventHandler CreateNewStudy;
        event EventHandler CreateNewProject;
        event EventHandler<string> OpenProject;
        event EventHandler SaveProject;
        event EventHandler SaveProjectAs;
        event EventHandler CloseProject;
        event EventHandler StartSimulation;
        event EventHandler StartAllSimulations;
        event EventHandler ShowCapacityCurves;


        event EventHandler<DockWindow> ActiveWindowChanged;
        event EventHandler Exit;
        event EventHandler ViewClosing;
    }
}