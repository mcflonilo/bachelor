using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using Gu.Units;
using SlimMessageBus;
using Ultra_Bend.Common.Data;

namespace Ultra_Bend.Common
{
    public static class ApplicationState
    {
        private static IContainer _container;
        private static Mapper _mapper;

        public static void RegisterDependencyResolver(IContainer container)
        {
            _container = container;

            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                
            }));
        }

        public static IContainer DependencyResolver => _container;

        public static IMessageBus MessageBus => _container.Resolve<IMessageBus>();

        public static Mapper Mapper => _mapper;

        // default SI units
        public static LengthUnit LengthUnit => LengthUnit.Metres;
        public static AngleUnit AngleUnit => AngleUnit.Radians;
        public static ForceUnit ForceUnit => ForceUnit.Kilonewtons;
        public static PressureUnit PressureUnit => PressureUnit.Kilopascals;
        public static TemperatureUnit TemperatureUnit => TemperatureUnit.Kelvin;
        public static DensityUnit DensityUnit => DensityUnit.KilogramsPerCubicMetre;

        // project data
        public static Project Project { get; set; } = new Project();
    }
}
