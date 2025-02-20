using System;
using System.Linq;
using System.Windows.Forms;
using Autofac;
using PostSharp.Patterns.Caching;
using PostSharp.Patterns.Caching.Backends;
using SlimMessageBus;
using SlimMessageBus.Host.Autofac;
using SlimMessageBus.Host.Config;
using SlimMessageBus.Host.Memory;
using Telerik.WinControls;
using Ultra_Bend.Common;

namespace Ultra_Bend
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // PostSharp related
            CachingServices.DefaultBackend = new MemoryCachingBackend();

            // SlimMessageBus related
            var builder = MessageBusBuilder.Create()
                .WithDependencyResolver(new AutofacMessageBusDependencyResolver())
                .WithProviderMemory(new MemoryMessageBusSettings()
                {
                    EnableMessageSerialization = false
                });

            var messageBus = builder.Build();

            // Autofac related

            var iocBuilder = new ContainerBuilder();

            iocBuilder.RegisterInstance(messageBus).SingleInstance();

            var container = iocBuilder.Build();
            
            // Setup application state
            ApplicationState.RegisterDependencyResolver(container);

            // Winforms related
            ThemeResolutionService.ApplicationThemeName = "TelerikMetroBlue";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}