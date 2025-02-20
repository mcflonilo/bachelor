using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gu.Units;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using UltraBend.Common;
using UltraBend.Databases;
using UltraBend.Databases.Material;
using UltraBend.Models;
using UltraBend.Presenters;
using UltraBend.Services;
using UltraBend.Services.Project;
using UltraBend.ViewForms;
using PostSharp.Patterns.Caching;
using PostSharp.Patterns.Caching.Backends;

namespace UltraBend
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CachingServices.DefaultBackend = new MemoryCachingBackend();

            ThemeResolutionService.ApplicationThemeName = "VisualStudio2012Light";
            //ThemeResolutionService.ApplicationThemeName = "Office2013Light";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ApplicationState.HasProject = false;
            ApplicationState.CurrentProjectTemporaryFile = null;

            ApplicationState.DensityUnit = DensityUnit.KilogramsPerCubicMetre;
            ApplicationState.PressureUnit = PressureUnit.Pascals;
            ApplicationState.TemperatureUnit = TemperatureUnit.Kelvin;
            ApplicationState.LengthUnit = LengthUnit.Metres;
            ApplicationState.AngleUnit = AngleUnit.Degrees;
            ApplicationState.ForceUnit = ForceUnit.Kilonewtons;

            // set default type sources (used by type converter in property grids which needs context free get ability)
            ApplicationState.DefaultTypeSources.Add(typeof(UltraBend.Services.DomainObjects.Material), (string filename) => new MaterialsService(filename).GetMaterials().Cast<IModelId>().ToList());

            using (var mainView = new MainView())
            {
                var presenter = new MainPresenter(mainView);
                Application.Run(presenter.View as RadRibbonForm);
            }
        }
    }
}
