using System;
using System.Linq;
using System.Windows;
using EDoc2.Sdk;
using EDoc2.ServiceProxy;
using EDoc2.ServiceProxy.Client;
using EDoc2.ServiceProxy.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectA.Core.Interfaces;
using ProjectA.Desktop.Services;
using ProjectA.Desktop.ViewModels;
using ProjectA.Infrastructure.Data;
using ProjectA.Infrastructure.Services;

namespace ProjectA.Desktop
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost Host;

        public App()
        {
            Host = CreateHostBuilder().Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // run hosted services
            await Host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((ctx, services) =>
                {
                    // register db and initialize
                    services.AddDbContextFactory<DocumentContext>(options =>
                        options.UseInMemoryDatabase("InMemory"));

                    var sp = services.BuildServiceProvider();
                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var dbContextFactory = scopedServices.GetRequiredService<IDbContextFactory<DocumentContext>>();
                        var context = dbContextFactory.CreateDbContext();
                        context.Database.EnsureCreated();
                    }

                    // register eDoc service
                    SdkBaseInfo.BaseUrl = "http://doc.scivic.com.cn:8889";
                    foreach (var type in AppDomain.CurrentDomain.GetAllTypes().Where(type =>
                                 type.IsInterface && typeof(IApplicationService).IsAssignableFrom(type)))
                        services.AddSingleton(type, builder =>
                        {
                            var client = new HttpApiClient(SdkBaseInfo.BaseUrl);
                            return ProxyGenerator.CreateInterfaceProxyWithoutTarget(type, client);
                        });

                    // register http service
                    services.AddHttpClient();

                    // register shepherd service
                    services.AddSingleton<ISynchronizeService, SynchronizeService>();
                    services.AddSingleton<ShepherdService>();
                    services.AddHostedService(provider => provider.GetService<ShepherdService>());

                    // register wpf
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<AddDocumentViewModel>();
                });
        }
    }
}