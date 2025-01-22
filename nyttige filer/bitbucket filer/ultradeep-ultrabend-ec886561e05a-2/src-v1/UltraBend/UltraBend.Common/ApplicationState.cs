using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UltraBend.Common;
using EventArgs = System.EventArgs;

namespace UltraBend
{
    public static class ApplicationState
    {
        /// <summary>
        /// HasProject
        /// </summary>
        private static bool _hasProject;
        public static bool HasProject
        {
            get => _hasProject;
            set
            {
                HasProjectChanged?.Invoke(null, EventArgs.Empty);
                _hasProject = value;
            }
        }

        public static event EventHandler HasProjectChanged;

        /// <summary>
        /// Has Changes
        /// </summary>
        private static bool _projectHasChanges;
        public static bool ProjectHasChanges
        {
            get => _projectHasChanges;
            set
            {
                ProjectHasChangesChanged?.Invoke(null, EventArgs.Empty);
                _projectHasChanges = value;
            }
        }
        public static event EventHandler ProjectHasChangesChanged;

        /// <summary>
        /// ProjectTemporaryFile
        /// </summary>
        private static string _currentProjectTemporaryFile;
        public static string CurrentProjectTemporaryFile
        {
            get => _currentProjectTemporaryFile;
            set
            {
                CurrentProjectTemporaryFileChanged?.Invoke(null, EventArgs.Empty);
                _currentProjectTemporaryFile = value;
            }
        }
        public static event EventHandler CurrentProjectTemporaryFileChanged;
        
        /// <summary>
        /// CurrentProjectFile
        /// </summary>
        private static string _currentProjectFile;
        public static string CurrentProjectFile
        {
            get => _currentProjectFile;
            set
            {
                CurrentProjectFileChanged?.Invoke(null, EventArgs.Empty);
                _currentProjectFile = value;
            }
        }
        public static event EventHandler CurrentProjectFileChanged;

        /// <summary>
        /// DefaultTypeSources
        /// </summary>
        private static Dictionary<Type, Func<string, List<IModelId>>> _defaultTypeSources = new Dictionary<Type, Func<string, List<IModelId>>>();

        public static Dictionary<Type, Func<string, List<IModelId>>> DefaultTypeSources
        {
            get => _defaultTypeSources;
            set
            {
                DefaultTypeSourcesChanged?.Invoke(null, EventArgs.Empty);
                _defaultTypeSources = value;
            }
        }

        public static event EventHandler DefaultTypeSourcesChanged;

        
        
        private static Gu.Units.DensityUnit _densityUnit;

        public static Gu.Units.DensityUnit DensityUnit
        {
            get => _densityUnit;
            set
            {
                DensityUnitChanged?.Invoke(null, EventArgs.Empty);
                _densityUnit = value;
            }
        }

        public static event EventHandler DensityUnitChanged;

        
        
        private static Gu.Units.TemperatureUnit _temperatureUnit;

        public static Gu.Units.TemperatureUnit TemperatureUnit
        {
            get => _temperatureUnit;
            set
            {
                TemperatureUnitChanged?.Invoke(null, EventArgs.Empty);
                _temperatureUnit = value;
            }
        }

        public static event EventHandler TemperatureUnitChanged;

        
        
        private static Gu.Units.PressureUnit _pressureUnit;

        public static Gu.Units.PressureUnit PressureUnit
        {
            get => _pressureUnit;
            set
            {
                PressureUnitChanged?.Invoke(null, EventArgs.Empty);
                _pressureUnit = value;
            }
        }

        public static event EventHandler PressureUnitChanged;
        
        
        
        private static Gu.Units.LengthUnit _lengthUnit;

        public static Gu.Units.LengthUnit LengthUnit
        {
            get => _lengthUnit;
            set
            {
                LengthUnitChanged?.Invoke(null, EventArgs.Empty);
                _lengthUnit = value;
            }
        }

        public static event EventHandler LengthUnitChanged;
        
        

        private static Gu.Units.AngleUnit _angleUnit;

        public static Gu.Units.AngleUnit AngleUnit
        {
            get => _angleUnit;
            set
            {
                AngleUnitChanged?.Invoke(null, EventArgs.Empty);
                _angleUnit = value;
            }
        }

        public static event EventHandler AngleUnitChanged;

        
        

        private static Gu.Units.ForceUnit _forceUnit;

        public static Gu.Units.ForceUnit ForceUnit
        {
            get => _forceUnit;
            set
            {
                ForceUnitChanged?.Invoke(null, EventArgs.Empty);
                _forceUnit = value;
            }
        }

        public static event EventHandler ForceUnitChanged;
        
        

    }
}
