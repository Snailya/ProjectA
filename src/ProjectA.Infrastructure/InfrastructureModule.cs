using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Autofac;
using EDoc2.Sdk;
using EDoc2.ServiceProxy;
using EDoc2.ServiceProxy.Client;
using EDoc2.ServiceProxy.DynamicProxy;
using MediatR;
using MediatR.Pipeline;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Infrastructure.Data;
using ProjectA.Infrastructure.Services;
using ProjectA.SharedKernel.Interfaces;
using Module = Autofac.Module;

namespace ProjectA.Infrastructure
{
    public class InfrastructureModule : Module
    {
        private readonly List<Assembly> _assemblies = new();
        private readonly bool _isDevelopment;

        public InfrastructureModule(bool isDevelopment, Assembly callingAssembly = null)
        {
            _isDevelopment = isDevelopment;

            var coreAssembly = Assembly.GetAssembly(typeof(Document));
            var infrastructureAssembly = Assembly.GetAssembly(typeof(EfRepository<>));

            _assemblies.Add(coreAssembly);
            _assemblies.Add(infrastructureAssembly);
            if (callingAssembly != null) _assemblies.Add(callingAssembly);
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (_isDevelopment)
                RegisterDevelopmentOnlyDependencies(builder);
            else
                RegisterProductionOnlyDependencies(builder);

            RegisterCommonDependencies(builder);
        }

        private void RegisterCommonDependencies(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(EfRepository<>))
                .As(typeof(IRepository<>))
                .As(typeof(IReadRepository<>))
                .InstancePerLifetimeScope();

            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            var mediatrOpenTypes = new[]
            {
                typeof(IRequestHandler<,>),
                typeof(IRequestExceptionHandler<,,>),
                typeof(IRequestExceptionAction<,>),
                typeof(INotificationHandler<>)
            };

            foreach (var mediatrOpenType in mediatrOpenTypes)
                builder
                    .RegisterAssemblyTypes(_assemblies.ToArray())
                    .AsClosedTypesOf(mediatrOpenType)
                    .AsImplementedInterfaces();
            
            SdkBaseInfo.BaseUrl = "http://doc.scivic.com.cn:8889";
            foreach (var type in AppDomain.CurrentDomain.GetAllTypes().Where(type =>
                         type.IsInterface && typeof(IApplicationService).IsAssignableFrom(type)))
                builder.Register(ctx =>
                        ProxyGenerator.CreateInterfaceProxyWithoutTarget(type, new HttpApiClient(SdkBaseInfo.BaseUrl)))
                    .As(type);
            builder.Register(c => new HttpClient())
                .As<HttpClient>();
            builder.RegisterType<EDocService>()
                .As<IFileSystemService>().InstancePerLifetimeScope();
        }

        private void RegisterDevelopmentOnlyDependencies(ContainerBuilder builder)
        {
            // TODO: Add development only services
        }

        private void RegisterProductionOnlyDependencies(ContainerBuilder builder)
        {
            // TODO: Add production only services
        }
    }
}