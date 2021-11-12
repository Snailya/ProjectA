using Autofac;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Services;

namespace ProjectA.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DocService>()
                .As<IDocService>().InstancePerLifetimeScope();
            builder.RegisterType<DocSetService>()
                .As<IDocSetService>().InstancePerLifetimeScope();
        }
    }
}