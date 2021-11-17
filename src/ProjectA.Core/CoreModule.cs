using Autofac;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Services;

namespace ProjectA.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DocumentHelper>()
                .As<IDocumentHelper>().InstancePerLifetimeScope();
            builder.RegisterType<AppService>()
                .As<IAppService>().InstancePerLifetimeScope();
        }
    }
}