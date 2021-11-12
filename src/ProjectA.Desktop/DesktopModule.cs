using Autofac;
using Microsoft.Extensions.Hosting;
using ProjectA.Desktop.Services;
using ProjectA.Desktop.ViewModels;

namespace ProjectA.Desktop
{
    public class DesktopModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ShepherdService>().AsSelf()
                .As<IHostedService>()
                .SingleInstance();
            
            // register wpf
            builder.RegisterType<MainWindowViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<AddDocumentViewModel>().SingleInstance();
        }
    }
}