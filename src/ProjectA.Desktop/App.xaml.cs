using System.Windows;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectA.Core;
using ProjectA.Desktop.ViewModels;
using ProjectA.Infrastructure;
using ProjectA.Infrastructure.Data;

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
            
            ViewModelLocator.MainWindowViewModel=Host.Services.GetService<MainWindowViewModel>();
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
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule(new CoreModule());
                    builder.RegisterModule(new InfrastructureModule(true));
                    builder.RegisterModule(new DesktopModule());
                })
                .ConfigureServices((ctx, services) =>
                {
                    // register db and initialize
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("InMemory"));

                    // var sp = services.BuildServiceProvider();
                    // using var scope = sp.CreateScope();
                    // var scopedServices = scope.ServiceProvider;
                    // var dbContextFactory = scopedServices.GetRequiredService<IDbContextFactory<AppDbContext>>();
                    // var context = dbContextFactory.CreateDbContext();
                    // context.Database.EnsureCreated();
                    
                    services.AddAutofac();
                    services.AddHttpClient();
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory());
        }
    }
}