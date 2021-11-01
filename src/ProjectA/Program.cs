using System;
using System.Linq;
using EDoc2.Sdk;
using EDoc2.ServiceProxy;
using EDoc2.ServiceProxy.Client;
using EDoc2.ServiceProxy.DynamicProxy;
using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectA.Data;
using ProjectA.Services;

namespace ProjectA
{
    internal class Program
    {
        private static ServiceProvider _serviceProvider;

        private static void Main(string[] args)
        {
            // build service provider
            _serviceProvider = ConfigureServices().BuildServiceProvider();

            // populate test data
            using var dbContext = new DocumentContext(
                _serviceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());
            SeedData.PopulateTestData(dbContext);

            // run hangfire background server
            GlobalConfiguration.Configuration.UseSQLiteStorage("Data Source=hangfire.db;");
            using var server = new BackgroundJobServer();
            RecurringJob.AddOrUpdate("shepherd-listen",
                () => _serviceProvider.GetRequiredService<ShepherdService>().ListenEDocServer(), Cron.Hourly);

            // run front end command handler
            _serviceProvider.GetRequiredService<CommandService>().Run();
        }


        private static ServiceCollection ConfigureServices()
        {
            // register database
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<DocumentContext>(options => options.UseSqlite("Data Source=document.db;"));

            // register eDoc service
            SdkBaseInfo.BaseUrl = "http://doc.scivic.com.cn:8889";
            foreach (var type in AppDomain.CurrentDomain.GetAllTypes().Where(type =>
                         type.IsInterface && typeof(IApplicationService).IsAssignableFrom(type)))
                serviceCollection.AddSingleton(type, builder =>
                {
                    var client = new HttpApiClient(SdkBaseInfo.BaseUrl);
                    return ProxyGenerator.CreateInterfaceProxyWithoutTarget(type, client);
                });

            // register shepherd service
            serviceCollection.AddSingleton<ShepherdService>();

            // register repository service
            serviceCollection.AddSingleton<RepositoryService>();

            // register command service
            serviceCollection.AddSingleton<CommandService>();

            return serviceCollection;
        }
    }
}