using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EDoc2.Sdk;
using EDoc2.ServiceProxy;
using EDoc2.ServiceProxy.Client;
using EDoc2.ServiceProxy.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectA.Core.Data;
using ProjectA.Core.Services;
using ProjectA.Desktop.ViewModels;
using ProjectA.Services;

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
            Host = CreateDefaultHost().Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // run hosted services
            await Host.RunAsync();
        }

        private static IHostBuilder CreateDefaultHost()
        {
            return new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                {
                })
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    builder
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", true, true)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((ctx, services) =>
                {
                    // register logging
                    services.AddLogging();

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
                    services.AddSingleton<ShepherdService>();

                    // register repository service
                    services.AddSingleton<RepositoryService>();

                    // register hosted services
                    // services.AddHostedService<CustomHostService>();

                    // register wpf
                    services.AddTransient<AddDocumentViewModel>();
                    
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<DashboardViewModel>();
                    services.AddSingleton<ToolbarViewModel>();

                });
        }
    }
}